// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflect.FacetFactory {
    public sealed class LifeCycleFunctionsFacetFactory : MethodPrefixBasedFacetFactoryAbstract, IMethodFilteringFacetFactory {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TitleMethodFacetFactory));

        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.PersistingMethod,
            RecognisedMethodsAndPrefixes.PersistedMethod,
            RecognisedMethodsAndPrefixes.UpdatingMethod,
            RecognisedMethodsAndPrefixes.UpdatedMethod
        };

        public LifeCycleFunctionsFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Objects, ReflectionType.Functional) { }

        public override string[] Prefixes => FixedPrefixes;

        #region IMethodFilteringFacetFactory Members

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var facets = new List<IFacet>();

            facets = AddPersistingFacet(facets, type, specification);
            facets = AddPersistedFacet(facets, type, specification);
            facets = AddUpdatingFacet(facets, type, specification);
            facets = AddUpdatedFacet(facets, type, specification);

            FacetUtils.AddFacets(facets);

            return metamodel;
        }

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) {
            return FixedPrefixes.Any(p => p == method.Name);
        }

        #endregion

        private static bool IsSameType(ParameterInfo pi, Type toMatch) {
            return pi != null &&
                   pi.ParameterType == toMatch;
        }

        private List<IFacet> AddLifecycleFacetsFacet(List<IFacet> facets, Type type, ISpecificationBuilder holder, string name) {
            var match = FunctionalIntrospector.Functions.SelectMany(t => t.GetMethods()).Where(m => m.Name == name).SingleOrDefault(m => IsSameType(m.GetParameters().FirstOrDefault(), type));
            var facet = match != null ? (IFacet) new PersistingCallbackFacetViaFunction(match, holder) : new PersistingCallbackFacetNull(holder);
            facets.Add(facet);
            return facets;
        }

        private List<IFacet> AddPersistingFacet(List<IFacet> facets, Type type, ISpecificationBuilder holder) {
            return AddLifecycleFacetsFacet(facets, type, holder, RecognisedMethodsAndPrefixes.PersistingMethod);
        }

        private List<IFacet> AddPersistedFacet(List<IFacet> facets, Type type, ISpecificationBuilder holder) {
            return AddLifecycleFacetsFacet(facets, type, holder, RecognisedMethodsAndPrefixes.PersistingMethod);
        }

        private List<IFacet> AddUpdatingFacet(List<IFacet> facets, Type type, ISpecificationBuilder holder) {
            return AddLifecycleFacetsFacet(facets, type, holder, RecognisedMethodsAndPrefixes.PersistingMethod);
        }

        private List<IFacet> AddUpdatedFacet(List<IFacet> facets, Type type, ISpecificationBuilder holder) {
            return AddLifecycleFacetsFacet(facets, type, holder, RecognisedMethodsAndPrefixes.PersistingMethod);
        }
    }
}