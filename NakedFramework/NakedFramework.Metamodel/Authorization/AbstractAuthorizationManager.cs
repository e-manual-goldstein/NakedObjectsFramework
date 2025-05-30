﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFramework.Metamodel.Authorization;

public abstract class AbstractAuthorizationManager : IAuthorizationManager {
    protected readonly Type defaultAuthorizer;
    protected readonly ImmutableDictionary<string, Type> namespaceAuthorizers = ImmutableDictionary<string, Type>.Empty;
    protected readonly ImmutableDictionary<string, Type> queryableActionAuthorizers = ImmutableDictionary<string, Type>.Empty;
    protected readonly ImmutableDictionary<string, Type> typeAuthorizers = ImmutableDictionary<string, Type>.Empty;

    protected AbstractAuthorizationManager(IAuthorizationConfiguration authorizationConfiguration, ILogger logger) {
        defaultAuthorizer = authorizationConfiguration.DefaultAuthorizer;
        if (defaultAuthorizer is null) {
            throw new InitialisationException(logger.LogAndReturn("Default Authorizer cannot be null"));
        }

        if (authorizationConfiguration.NamespaceAuthorizers.Any()) {
            namespaceAuthorizers = authorizationConfiguration.NamespaceAuthorizers.ToImmutableDictionary();
        }

        if (authorizationConfiguration.TypeAuthorizers.Any()) {
            typeAuthorizers = authorizationConfiguration.TypeAuthorizers.ToImmutableDictionary();
        }

        if (authorizationConfiguration.QueryableActionAuthorizers.Any()) {
            queryableActionAuthorizers = authorizationConfiguration.QueryableActionAuthorizers.ToImmutableDictionary();
        }
    }

    public abstract bool IsVisible(INakedFramework framework, INakedObjectAdapter target, IIdentifier identifier);
    public abstract bool IsEditable(INakedFramework framework, INakedObjectAdapter target, IIdentifier identifier);

    protected abstract object CreateAuthorizer(Type type, ILifecycleManager lifecycleManager);

    private static bool IsGenericCollection(INakedObjectAdapter target, INakedFramework framework, out string name) {
        name = target.Spec.IsCollection ? target.Spec.GetFacet<ITypeOfFacet>()?.GetValueSpec(target, framework.MetamodelManager.Metamodel).FullName : "";
        return !string.IsNullOrEmpty(name);
    }

    protected object GetAuthorizer(INakedObjectAdapter target, INakedFramework framework) {
        Type GetObjectAuthorizer() {
            //Look for exact-fit TypeAuthorizer
            // order here as ImmutableDictionary not ordered
            var fullyQualifiedOfTarget = target.Spec.FullName;
            return typeAuthorizers.Where(ta => ta.Key == fullyQualifiedOfTarget).Select(ta => ta.Value).FirstOrDefault() ??
                   namespaceAuthorizers.OrderByDescending(x => x.Key.Length).Where(x => fullyQualifiedOfTarget.StartsWith(x.Key)).Select(x => x.Value).FirstOrDefault() ??
                   defaultAuthorizer;
        }

        Type GetCollectionAuthorizer(string fullyQualifiedOfTarget) {
            return queryableActionAuthorizers.Where(ta => ta.Key == fullyQualifiedOfTarget).Select(ta => ta.Value).FirstOrDefault() ??
                   defaultAuthorizer;
        }

        var authorizer = IsGenericCollection(target, framework, out var name) ? GetCollectionAuthorizer(name) : GetObjectAuthorizer();

        return CreateAuthorizer(authorizer, framework.LifecycleManager);
    }
}