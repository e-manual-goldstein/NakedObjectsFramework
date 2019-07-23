// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Immutable;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflect.FacetFactory {
    public sealed class TitleFunctionFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TitleMethodFacetFactory));

        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.TitleMethod
        };

        public TitleFunctionFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Actions, ReflectionType.Functional) { }

        public override string[] Prefixes => FixedPrefixes;

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod, IMethodRemover methodRemover, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            RemoveMethod(methodRemover, actionMethod);

            if (actionMethod.IsStatic) {
                var parameter = actionMethod.GetParameters()[0];

                var result = reflector.LoadSpecification(parameter.ParameterType, metamodel);
                metamodel = result.Item2;
                var onSpec = result.Item1 as IObjectSpecBuilder;

                var facet = new TitleFacetViaTitleFunction(actionMethod, onSpec);
                FacetUtils.AddFacet(facet);
            }

            return metamodel;
        }
    }
}