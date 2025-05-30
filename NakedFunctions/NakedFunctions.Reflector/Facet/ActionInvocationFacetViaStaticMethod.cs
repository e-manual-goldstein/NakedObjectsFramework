// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Error;
using NakedFramework.Core.Persist;
using NakedFramework.Error;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Serialization;
using NakedFunctions.Reflector.Component;

namespace NakedFunctions.Reflector.Facet;

[Serializable]
public sealed class ActionInvocationFacetViaStaticMethod : ActionInvocationFacetAbstract, IImperativeFacet {
    private readonly TypeSerializationWrapper elementType;
    private readonly MethodSerializationWrapper methodWrapper;
    private readonly TypeSerializationWrapper onType;

    private readonly int paramCount;
    private readonly TypeSerializationWrapper returnType;

    public ActionInvocationFacetViaStaticMethod(MethodInfo method,
                                                Type onType,
                                                Type returnType,
                                                Type elementType,
                                                bool isQueryOnly,
                                                ILogger<ActionInvocationFacetViaStaticMethod> logger) {
        methodWrapper = SerializationFactory.Wrap(method, logger);
        paramCount = method.GetParameters().Length;
        this.onType = onType is not null ? SerializationFactory.Wrap(onType) : null;
        this.returnType = returnType is not null ? SerializationFactory.Wrap(returnType) : null;
        this.elementType = elementType is not null ? SerializationFactory.Wrap(elementType) : null;
        IsQueryOnly = isQueryOnly;
    }

    public override Type ReturnType => returnType?.Type;

    public override Type OnType => onType?.Type;

    public override Type ElementType => elementType?.Type;

    public override bool IsQueryOnly { get; }

    private static INakedObjectAdapter AdaptResult(INakedObjectManager nakedObjectManager, object result) =>
        result is null ? null : nakedObjectManager.CreateAdapter(result, null, null);

    private static Func<IDictionary<object, object>, bool> GetPostSaveFunction(FunctionalContext functionalContext, INakedFramework framework) {
        var postSaveFunction = functionalContext.PostSaveFunction;

        if (postSaveFunction is not null) {
            return map => {
                var newContext = new FunctionalContext { Persistor = functionalContext.Persistor, Provider = functionalContext.Provider, ProxyMap = map };
                var innerContext = (FunctionalContext)postSaveFunction(newContext);
                var updated = PersistResult(framework.LifecycleManager, innerContext.New, innerContext.Deleted, innerContext.Updated, GetPostSaveFunction(innerContext, framework));
                return updated.Any();
            };
        }

        return _ => false;
    }

    private static (object original, object updated)[] PersistResult(ILifecycleManager lifecycleManager, object[] newObjects, object[] deletedObjects, (object proxy, object updated)[] updatedObjects, Func<IDictionary<object, object>, bool> postSaveFunction) =>
        lifecycleManager.Persist(new DetachedObjects(newObjects, deletedObjects, updatedObjects, postSaveFunction)).ToArray();

    private static (object, FunctionalContext) CastTuple(ITuple tuple) => (tuple[0], (FunctionalContext)tuple[1]);

    private static void HandleErrors(FunctionalContext functionalContext) {
        var errors = functionalContext.Errors;
        if (errors.Any()) {
            throw new NakedObjectDomainException("", new AggregateException(errors));
        }
    }

    private static (object original, object updated)[] HandleContext(FunctionalContext functionalContext, INakedFramework framework) {
        HandleErrors(functionalContext);
        return PersistResult(framework.LifecycleManager, functionalContext.New, functionalContext.Deleted, functionalContext.Updated, GetPostSaveFunction(functionalContext, framework));
    }

    private static object HandleTupleResult((object, FunctionalContext) tuple, INakedFramework framework) {
        var (toReturn, context) = tuple;
        var allPersisted = HandleContext(context, framework);

        foreach (var (original, updated) in allPersisted) {
            if (ReferenceEquals(original, toReturn)) {
                return updated;
            }
        }

        return toReturn;
    }

    private static object HandleContextResult(FunctionalContext functionalContext, INakedFramework framework) {
        HandleContext(functionalContext, framework);
        return null;
    }

    private INakedObjectAdapter HandleInvokeResult(INakedFramework framework, object result) {
        // if any changes made by invocation fail 

        if (framework.Persistor.HasChanges()) {
            throw new PersistFailedException($"method {GetMethod()} on {GetMethod().DeclaringType} made database changes and so is not pure");
        }

        var toReturn = result switch {
            ITuple tuple => HandleTupleResult(CastTuple(ValidateTuple(tuple)), framework),
            FunctionalContext context => HandleContextResult(context, framework),
            _ => result
        };

        return AdaptResult(framework.NakedObjectManager, toReturn);
    }

    private ITuple ValidateTuple(ITuple tuple) {
        var size = tuple.Length;

        if (size is not 2) {
            throw new InvokeException($"Invalid return type {size} item tuple on {GetMethod().Name}");
        }

        return tuple;
    }

    public override INakedObjectAdapter Invoke(INakedObjectAdapter inObjectAdapter,
                                               INakedObjectAdapter[] parameters,
                                               INakedFramework framework) {
        if (parameters.Length != paramCount) {
            throw new NakedObjectSystemException($"{GetMethod()} requires {paramCount} parameters, not {parameters.Length}");
        }

        var rawParms = parameters.Select(p => p?.Object).ToArray();

        return HandleInvokeResult(framework, methodWrapper.Invoke<object>(rawParms));
    }

    public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter,
                                               INakedObjectAdapter[] parameters,
                                               int resultPage,
                                               INakedFramework framework) =>
        Invoke(nakedObjectAdapter, parameters, framework);

    #region IImperativeFacet Members

    /// <summary>
    ///     See <see cref="IImperativeFacet" />
    /// </summary>
    public override MethodInfo GetMethod() => methodWrapper.GetMethod();

    public override Func<object, object[], object> GetMethodDelegate() => methodWrapper.GetMethodDelegate();

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.