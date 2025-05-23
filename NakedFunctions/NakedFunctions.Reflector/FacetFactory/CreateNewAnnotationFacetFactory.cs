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
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedFunctions.Reflector.Utils;

namespace NakedFunctions.Reflector.FacetFactory;

public sealed class CreateNewAnnotationFacetFactory : FunctionalFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    private readonly ILogger<CreateNewAnnotationFacetFactory> logger;

    public CreateNewAnnotationFacetFactory(IFacetFactoryOrder<CreateNewAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Actions) =>
        logger = loggerFactory.CreateLogger<CreateNewAnnotationFacetFactory>();

    private static bool IsCollectionOrNull(Type type) =>
        type is null ||
        CollectionUtils.IsGenericEnumerable(type) ||
        type.IsArray ||
        CollectionUtils.IsCollectionButNotArray(type);

    private static Type ToCreateType(MethodInfo method) {
        var returnType = FacetUtils.IsTuple(method.ReturnType) ? method.ReturnType.GetGenericArguments().First() : null;
        return IsCollectionOrNull(returnType) ? null : returnType;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        if (method.IsDefined(typeof(CreateNewAttribute), false)) {
            var toCreateType = ToCreateType(method);

            if (toCreateType is not null && FactoryUtils.MatchParmsAndProperties(method, toCreateType, logger).Any()) {
                FacetUtils.AddFacet(new CreateNewFacet(toCreateType), specification);
            }
        }

        return metamodel;
    }
}