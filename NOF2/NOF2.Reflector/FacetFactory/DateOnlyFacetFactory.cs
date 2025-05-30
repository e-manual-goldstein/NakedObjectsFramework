// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NOF2.Reflector.Helpers;
using NOF2.ValueHolder;

namespace NOF2.Reflector.FacetFactory;

public sealed class DateOnlyFacetFactory : AbstractNOF2FacetFactoryProcessor {
    public DateOnlyFacetFactory(IFacetFactoryOrder<DateOnlyFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.PropertiesAndActionParameters) { }

    private static void Process(MemberInfo member, ISpecificationBuilder holder) {
        var dataTypeAttribute = member.GetCustomAttribute<DataTypeAttribute>();
        var concurrencyCheckAttribute = member.GetCustomAttribute<ConcurrencyCheckAttribute>();
        FacetUtils.AddFacet(Create(dataTypeAttribute, concurrencyCheckAttribute), holder);
    }

    private static bool IsDate(Type type) {
        var valueHolderType = NOF2Helpers.IsOrImplementsValueHolder(type);
        return (valueHolderType == typeof(DateTime) || valueHolderType == typeof(DateTime?)) && type.IsAssignableTo(typeof(IDateOnly));
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        if (IsDate(property.PropertyType)) {
            Process(property, specification);
        }

        return metamodel;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var parameter = method.GetParameters()[paramNum];

        if (IsDate(parameter.ParameterType)) {
            var dataTypeAttribute = parameter.GetCustomAttribute<DataTypeAttribute>();
            var concurrencyCheckAttribute = parameter.GetCustomAttribute<ConcurrencyCheckAttribute>();
            FacetUtils.AddFacet(Create(dataTypeAttribute, concurrencyCheckAttribute), holder);
        }

        return metamodel;
    }

    private static IDateOnlyFacet Create(DataTypeAttribute attribute, ConcurrencyCheckAttribute concurrencyCheckAttribute) =>
        attribute?.DataType is DataType.Date
            ? DateOnlyFacet.Instance
            : concurrencyCheckAttribute is not null
                ? null
                : attribute?.DataType is DataType.DateTime
                    ? null
                    : DateOnlyFacet.Instance;
}