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
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflect.FacetFactory {
    /// <summary>
    ///     Creates an <see cref="IContributedActionFacet" /> based on the presence of an
    ///     <see cref="ContributedActionAttribute" /> annotation
    /// </summary>
    public sealed class ContributedActionAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ContributedActionAnnotationFacetFactory));

        public ContributedActionAnnotationFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Actions) { }

        private ImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo member, ISpecification holder, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var allParams = member.GetParameters();
            var paramsWithAttribute = allParams.Where(p => p.GetCustomAttribute<ContributedActionAttribute>() != null).ToArray();
            if (!paramsWithAttribute.Any()) return metamodel; //Nothing to do
            var facet = new ContributedActionFacet(holder);
            foreach (ParameterInfo p in paramsWithAttribute) {
                var attribute = p.GetCustomAttribute<ContributedActionAttribute>();
                var result = reflector.LoadSpecification(p.ParameterType, metamodel);
                metamodel = result.Item2;

                var type = result.Item1 as IObjectSpecImmutable;
                if (type != null) {
                    if (type.IsParseable) {
                        Log.WarnFormat("ContributedAction attribute added to a value parameter type: {0}", member.Name);
                    }
                    else if (type.IsCollection) {
                        result = reflector.LoadSpecification(member.DeclaringType, metamodel);
                        metamodel = result.Item2;
                        var parent = result.Item1 as IObjectSpecImmutable;

                        if (parent is IObjectSpecBuilder) {
                            metamodel = AddLocalCollectionContributedAction(reflector, p, facet, metamodel);
                        }
                        else {
                            metamodel = AddCollectionContributedAction(reflector, member, type, p, facet, attribute, metamodel);
                        }
                    }
                    else {
                        facet.AddObjectContributee(type, attribute.SubMenu, attribute.Id);
                    }
                }
            }

            FacetUtils.AddFacet(facet);
            return metamodel;
        }

        private static ImmutableDictionary<string, ITypeSpecBuilder> AddCollectionContributedAction(IReflector reflector, MethodInfo member, IObjectSpecImmutable type, ParameterInfo p, ContributedActionFacet facet, ContributedActionAttribute attribute, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (!type.IsQueryable) {
                Log.WarnFormat("ContributedAction attribute added to a collection parameter type other than IQueryable: {0}", member.Name);
            }
            else {
                var result = reflector.LoadSpecification(member.ReturnType, metamodel);
                metamodel = result.Item2;
                var returnType = result.Item1 as IObjectSpecImmutable;
                if (returnType.IsCollection) {
                    Log.WarnFormat("ContributedAction attribute added to an action that returns a collection: {0}", member.Name);
                }
                else {
                    Type elementType = p.ParameterType.GetGenericArguments()[0];
                    result = reflector.LoadSpecification(elementType, metamodel);
                    metamodel = result.Item2;
                    type = result.Item1 as IObjectSpecImmutable;
                    facet.AddCollectionContributee(type, attribute.SubMenu, attribute.Id);
                }
            }

            return metamodel;
        }

        private static ImmutableDictionary<string, ITypeSpecBuilder> AddLocalCollectionContributedAction(IReflector reflector, ParameterInfo p, ContributedActionFacet facet, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Type elementType = p.ParameterType.GetGenericArguments()[0];
            var result = reflector.LoadSpecification(elementType, metamodel);
            metamodel = result.Item2;
            var type = result.Item1 as IObjectSpecImmutable;
            facet.AddLocalCollectionContributee(type, p.Name);
            return metamodel;
        }

        public override ImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            return Process(reflector, method, specification, metamodel);
        }
    }
}