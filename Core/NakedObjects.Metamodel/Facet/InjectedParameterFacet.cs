// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class InjectedParameterFacet : FacetAbstract, IInjectedParameterFacet {
        private readonly Type typeOfQueryable;

        public InjectedParameterFacet(ISpecification holder, Type typeOfQueryable)
            : base(Type, holder) {
            this.typeOfQueryable = typeOfQueryable;
        }

        public static Type Type => typeof(IInjectedParameterFacet);

        #region IInjectedParameterFacet Members

        public object GetInjectedValue(INakedObjectsFramework framework) {
            var f = GetType().GetMethod("GetInjectedQueryable")?.MakeGenericMethod(typeOfQueryable);
            return f?.Invoke(this, new object[] {framework});
        }

        #endregion

        // ReSharper disable once UnusedMember.Global
        public IQueryable<T> GetInjectedQueryable<T>(INakedObjectsFramework framework) where T : class {
            return framework.Persistor.Instances<T>();
        }
    }
}