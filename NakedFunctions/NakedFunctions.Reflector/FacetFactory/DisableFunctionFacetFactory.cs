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
using Microsoft.Extensions.Logging;
using NakedFunctions.Meta.Facet;
using NakedObjects;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflector.FacetFactory;

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class DisableFunctionFacetFactory : FunctionalFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IMethodFilteringFacetFactory {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.DisablePrefix
        };

        private readonly ILogger<DisableFunctionFacetFactory> logger;

        public DisableFunctionFacetFactory(IFacetFactoryOrder<DisableFunctionFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions) =>
            logger = loggerFactory.CreateLogger<DisableFunctionFacetFactory>();

        public string[] Prefixes => FixedPrefixes;

        private static bool IsSameType(ParameterInfo pi, Type toMatch) =>
            pi is not null &&
            pi.ParameterType == toMatch;

        private static bool NameMatches(MethodInfo compFunction, MethodInfo actionFunction) =>
            compFunction.Name.StartsWith(RecognisedMethodsAndPrefixes.DisablePrefix)
            && compFunction.Name.Substring(RecognisedMethodsAndPrefixes.DisablePrefix.Length) == actionFunction.Name;

        #region IMethodFilteringFacetFactory Members

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod,  ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var parameterType = actionMethod.GetParameters().FirstOrDefault()?.ParameterType;

            if (parameterType is not null) {
                var declaringType = actionMethod.DeclaringType;
                // find matching disable function
                var match = declaringType?.GetMethods().SingleOrDefault(m => NameMatches(m, actionMethod) && IsSameType(m.GetParameters().FirstOrDefault(), parameterType));

                if (match is not null) {
                    FacetUtils.AddFacet(new DisableForContextViaFunctionFacet(match, action));
                }
            }

            return metamodel;
        }

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) => method.Name.StartsWith(RecognisedMethodsAndPrefixes.DisablePrefix);

        #endregion
    }
}