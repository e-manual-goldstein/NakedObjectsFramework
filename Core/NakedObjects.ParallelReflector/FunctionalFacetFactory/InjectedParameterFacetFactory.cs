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
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflect.FacetFactory {
    /// <summary>
    ///     Sets up all the <see cref="IFacet" />s for an action in a single shot
    /// </summary>
    public sealed class InjectedParameterFacetFactory : FacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ActionMethodsFacetFactory));

        public InjectedParameterFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.ActionParameters, ReflectionType.Functional) { }

        public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            // for the moment just inject all queryable parameters 

            var parm = method.GetParameters()[paramNum];

            if (CollectionUtils.IsQueryable(parm.ParameterType)) {
                var facet = new InjectedParameterFacet(holder);
                FacetUtils.AddFacet(facet);
            }

            return metamodel;
        }
    }
}