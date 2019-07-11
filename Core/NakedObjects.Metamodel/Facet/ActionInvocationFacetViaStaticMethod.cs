// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using System.Runtime.Serialization;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class ActionInvocationFacetViaStaticMethod : ActionInvocationFacetAbstract, IImperativeFacet {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ActionInvocationFacetViaMethod));

        [field: NonSerialized] private readonly MethodInfo actionMethod;

        private readonly int paramCount;

        public ActionInvocationFacetViaStaticMethod(MethodInfo method, ITypeSpecImmutable onType, IObjectSpecImmutable returnType, IObjectSpecImmutable elementType, ISpecification holder, bool isQueryOnly)
            : base(holder) {
            actionMethod = method;
            paramCount = method.GetParameters().Length;
            this.OnType = onType;
            this.ReturnType = returnType;
            this.ElementType = elementType;
            this.IsQueryOnly = isQueryOnly;
        }

        public override MethodInfo ActionMethod => actionMethod;

        public override IObjectSpecImmutable ReturnType { get; }

        public override ITypeSpecImmutable OnType { get; }

        public override IObjectSpecImmutable ElementType { get; }

        public override bool IsQueryOnly { get; }

        #region IImperativeFacet Members

        /// <summary>
        ///     See <see cref="IImperativeFacet" />
        /// </summary>
        public MethodInfo GetMethod() {
            return actionMethod;
        }

        public Func<object, object[], object> GetMethodDelegate() {
            return null;
        }

        #endregion

        private INakedObjectAdapter AdaptResult(INakedObjectManager nakedObjectManager, object result) {
            INakedObjectAdapter adaptedResult = nakedObjectManager.CreateAdapterForExistingObject(result);
            return adaptedResult;
        }

        private void PersistResult(ILifecycleManager lifecycleManager, object result) {
            lifecycleManager.Persist(result);
        }

        private INakedObjectAdapter HandleInvokeResult(INakedObjectManager nakedObjectManager, ILifecycleManager lifecycleManager, object result) {
            var type = result.GetType();
            if (FacetUtils.IsTuple(type) || FacetUtils.IsValueTuple(type)) {
                // TODO dynamic just for spike do a proper cast in real code
                dynamic tuple = result;
                PersistResult(lifecycleManager, tuple.Item2);
                return AdaptResult(nakedObjectManager, tuple.Item1);
            }

            return AdaptResult(nakedObjectManager, result);
        }


        public override INakedObjectAdapter Invoke(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter[] parameters, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager, IMessageBroker messageBroker, ITransactionManager transactionManager) {
            if (parameters.Length != paramCount) {
                Log.Error(actionMethod + " requires " + paramCount + " parameters, not " + parameters.Length);
            }

            return HandleInvokeResult(nakedObjectManager, lifecycleManager,InvokeUtils.InvokeStatic(actionMethod, parameters));
        }

        public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, int resultPage, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager, IMessageBroker messageBroker, ITransactionManager transactionManager) {
            return Invoke(nakedObjectAdapter, parameters, lifecycleManager, manager, session, nakedObjectManager, messageBroker, transactionManager);
        }

        protected override string ToStringValues() {
            return "method=" + actionMethod;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) { }
    }

    // Copyright (c) Naked Objects Group Ltd.
}