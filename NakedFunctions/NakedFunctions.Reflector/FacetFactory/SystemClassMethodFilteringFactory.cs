﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Util;

namespace NakedFunctions.Reflector.FacetFactory {
    /// <summary>
    ///     This factory filters out actions on system types. So for example 'GetHashCode' will not show up when displaying a
    ///     string.
    /// </summary>
    public sealed class SystemClassMethodFilteringFactory : FunctionalFacetFactoryProcessor, IMethodFilteringFacetFactory {
        private ILogger<SystemClassMethodFilteringFactory> logger;

        public SystemClassMethodFilteringFactory(IFacetFactoryOrder<SystemClassMethodFilteringFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions) =>
            logger = loggerFactory.CreateLogger<SystemClassMethodFilteringFactory>();

        #region IMethodFilteringFacetFactory Members

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) => TypeKeyUtils.IsSystemClass(method.DeclaringType);

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}