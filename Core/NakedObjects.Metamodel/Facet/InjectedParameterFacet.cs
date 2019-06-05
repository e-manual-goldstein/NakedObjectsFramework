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
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class InjectedParameterFacet : FacetAbstract, IInjectedParameterFacet {
        private readonly Type typeOfQueryable;
        private readonly Type mappedType;

        public InjectedParameterFacet(ISpecification holder, Type typeOfQueryable)
            : base(Type, holder) {
            this.typeOfQueryable = typeOfQueryable;
            mappedType = MapType(typeOfQueryable);
        }

        private static string ShortName(Type t) {
            return TypeNameUtils.GetShortName(t.FullName);
        }

        private static Type MapType(Type typeOfQueryable) {
            var assembly = typeOfQueryable.Assembly;
            return assembly.GetTypes().SingleOrDefault(t => ShortName(t) == ShortName(typeOfQueryable) && t.FullName != typeOfQueryable.FullName);
        }

        private static object MapInstance(object instance, Type newType) {
            // hard code for moment
            var v = instance.GetType().GetProperty("ProductId")?.GetValue(instance);
            return Activator.CreateInstance(newType, v);
        }


        public static Type Type => typeof(IInjectedParameterFacet);

        #region IInjectedParameterFacet Members

        public IQueryable<T> GetInjectedValue<T, TU>(INakedObjectsFramework framework) {
            return framework.Persistor.Instances(mappedType).Cast<TU>().Select(i => (T)MapInstance(i, typeOfQueryable));
        }

        #endregion

        public object GetInjectedValue(INakedObjectsFramework framework) {
            var f = GetType().GetMethod("GetInjectedValue")?.MakeGenericMethod(typeOfQueryable, mappedType);
            return f?.Invoke(this, new object [] {framework});
        }
    }
}