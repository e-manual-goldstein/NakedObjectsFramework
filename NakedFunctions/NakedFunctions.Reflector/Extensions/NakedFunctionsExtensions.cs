﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.DependencyInjection;
using NakedFunctions.Reflector.Component;
using NakedFunctions.Reflector.Configuration;
using NakedFunctions.Reflector.FacetFactory;
using NakedFunctions.Reflector.Reflect;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.DependencyInjection.DependencyInjection;
using NakedObjects.DependencyInjection.Extensions;

namespace NakedFunctions.Reflector.Extensions {
    public static class NakedFunctionsExtensions {
        public static FunctionalReflectorConfiguration FunctionalReflectorConfig(NakedFunctionsOptions options) =>
            new FunctionalReflectorConfiguration(options.FunctionalTypes, options.Functions, options.ConcurrencyCheck);


        public static void AddNakedFunctions(this NakedCoreOptions coreOptions, Action<NakedFunctionsOptions> setupAction) {
            var options = new NakedFunctionsOptions();
            setupAction(options);

            options.RegisterCustomTypes?.Invoke(coreOptions.Services);

            ParallelConfig.RegisterWellKnownServices(coreOptions.Services);
            coreOptions.Services.RegisterFacetFactories<IFunctionalFacetFactoryProcessor>(FunctionalFacetFactories.StandardFacetFactories());
            coreOptions.Services.AddSingleton<FunctionalFacetFactorySet, FunctionalFacetFactorySet>();
            coreOptions.Services.AddSingleton<FunctionClassStrategy, FunctionClassStrategy>();
            coreOptions.Services.AddSingleton<IReflector, FunctionalReflector>();
            coreOptions.Services.AddSingleton<IFunctionalReflectorConfiguration>(p => FunctionalReflectorConfig(options));
        }
    }
}