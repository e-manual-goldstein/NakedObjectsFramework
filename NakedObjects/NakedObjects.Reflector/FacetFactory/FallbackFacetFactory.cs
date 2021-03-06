// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflector.FacetFactory {
    /// <summary>
    ///     Central point for providing some kind of default for any  <see cref="IFacet" />s required by the Naked Objects
    ///     Framework itself.
    /// </summary>
    public sealed class FallbackFacetFactory : ObjectFacetFactoryProcessor {
        public FallbackFacetFactory(IFacetFactoryOrder<FallbackFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Everything) { }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector,  Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            FacetUtils.AddFacets(
                new IFacet[] {
                    new DescribedAsFacetNone(specification),
                    new ImmutableFacetNever(specification),
                    new TitleFacetNone(specification)
                });
            return metamodel;
        }

        private static void Process(ISpecification holder) {
            var facets = new List<IFacet>();

            if (holder is IMemberSpecImmutable specImmutable) {
                facets.Add(new NamedFacetInferred(specImmutable.Identifier.MemberName, holder));
                facets.Add(new DescribedAsFacetNone(holder));
            }

            if (holder is IAssociationSpecImmutable) {
                facets.Add(new ImmutableFacetNever(holder));
                facets.Add(new PropertyDefaultFacetNone(holder));
                facets.Add(new PropertyValidateFacetNone(holder));
            }

            if (holder is IOneToOneAssociationSpecImmutable immutable) {
                facets.Add(new MaxLengthFacetZero(holder));
                DefaultTypicalLength(facets, immutable.ReturnSpec, immutable);
                facets.Add(new MultiLineFacetNone(holder));
            }

            if (holder is IActionSpecImmutable) {
                facets.Add(new ExecutedFacetDefault(holder));
                facets.Add(new ActionDefaultsFacetNone(holder));
                facets.Add(new ActionChoicesFacetNone(holder));
                facets.Add(new PageSizeFacetDefault(holder));
            }

            FacetUtils.AddFacets(facets);
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector,  MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Process(specification);
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector,  PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Process(specification);
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector,  MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var facets = new List<IFacet>();

            if (holder is IActionParameterSpecImmutable param) {
                var name = method.GetParameters()[paramNum].Name ?? method.GetParameters()[paramNum].ParameterType.FullName;
                INamedFacet namedFacet = new NamedFacetInferred(name, holder);
                facets.Add(namedFacet);
                facets.Add(new DescribedAsFacetNone(holder));
                facets.Add(new MultiLineFacetNone(holder));
                facets.Add(new MaxLengthFacetZero(holder));
                facets.Add(new TypicalLengthFacetZero(holder));
                DefaultTypicalLength(facets, param.Specification, param);
            }

            FacetUtils.AddFacets(facets);
            return metamodel;
        }

        private static void DefaultTypicalLength(ICollection<IFacet> facets, ISpecification specification, ISpecification holder) {
            var typicalLengthFacet = specification.GetFacet<ITypicalLengthFacet>();
            if (typicalLengthFacet is null) {
                typicalLengthFacet = new TypicalLengthFacetZero(holder);
            }
            else {
                typicalLengthFacet = new TypicalLengthFacetInferred(typicalLengthFacet.Value, holder);
            }

            facets.Add(typicalLengthFacet);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}