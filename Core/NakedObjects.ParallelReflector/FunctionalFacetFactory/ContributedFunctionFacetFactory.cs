// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedFunctions;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.Service;

namespace NakedObjects.ParallelReflect.FacetFactory {
    /// <summary>
    ///     Creates an <see cref="IContributedActionFacet" /> based on the presence of an
    ///     <see cref="ContributedActionAttribute" /> annotation
    /// </summary>
    public sealed class ContributedFunctionFacetFactory : FacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ContributedFunctionFacetFactory));

        public ContributedFunctionFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Actions, ReflectionType.Functional) { }

        private static Type GetContributeeType(ParameterInfo p) {
            // temp - need better way to id Menu Functions
            return p == null ||
                   p.ParameterType.IsValueType ||
                   p.GetCustomAttribute<InjectedAttribute>() != null ? typeof(MenuService) : p.ParameterType;
        }

        private IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo member, ISpecification holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            // all functions are contributed to first parameter or MenuService

            var allParams = member.GetParameters();

            var facet = new ContributedFunctionFacet(holder);

            var contributeeType = GetContributeeType(allParams.FirstOrDefault());
            var result = reflector.LoadSpecification(contributeeType, metamodel);
            metamodel = result.Item2;

            var type = result.Item1 as ITypeSpecImmutable;
            if (type != null) {
                facet.AddContributee(type);
            }

            FacetUtils.AddFacet(facet);
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            return Process(reflector, method, specification, metamodel);
        }
    }
}