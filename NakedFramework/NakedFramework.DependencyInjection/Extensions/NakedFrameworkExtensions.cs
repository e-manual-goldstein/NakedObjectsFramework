﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Core.Configuration;
using NakedObjects.DependencyInjection.DependencyInjection;
using NakedObjects.Facade;
using NakedObjects.Facade.Impl;
using NakedObjects.Facade.Impl.Implementation;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Facade.Interface;
using NakedObjects.Facade.Translation;
using NakedObjects.Meta.Audit;
using NakedObjects.Meta.Authorization;
using NakedObjects.Meta.Profile;

namespace NakedObjects.DependencyInjection.Extensions {
    public static class NakedFrameworkExtensions {
        public static CoreConfiguration CoreConfig(NakedCoreOptions options) {
            var config = new CoreConfiguration(options.MainMenus);

            if (options.SupportedSystemTypes is not null) {
                config.SupportedSystemTypes = options.SupportedSystemTypes(config.SupportedSystemTypes.ToArray()).ToList();
            }

            return config;
        }

        private static void AddNakedCoreFramework(this IServiceCollection services, NakedCoreOptions options) {
            ParallelConfig.RegisterCoreSingletonTypes(services);
            ParallelConfig.RegisterCoreScopedTypes(services);

            if (options.AuthorizationConfiguration is not null) {
                services.AddSingleton(options.AuthorizationConfiguration);
                services.AddSingleton<IFacetDecorator, AuthorizationManager>();
            }

            if (options.AuditConfiguration is not null) {
                services.AddSingleton(options.AuditConfiguration);
                services.AddSingleton<IFacetDecorator, AuditManager>();
            }

            if (options.ProfileConfiguration is not null)
            {
                services.AddSingleton(options.ProfileConfiguration);
                services.AddSingleton<IFacetDecorator, ProfileManager>();
            }


            services.AddSingleton<ICoreConfiguration>(p => CoreConfig(options));


            // frameworkFacade
            services.AddTransient<IOidTranslator, OidTranslatorSlashSeparatedTypeAndIds>();
            services.AddTransient<IOidStrategy, EntityOidStrategy>();
            services.AddTransient<IStringHasher, InvariantStringHasher>();
            services.AddTransient<IFrameworkFacade, FrameworkFacade>();

            //Externals
            services.AddScoped<IPrincipal>(p => p.GetService<IHttpContextAccessor>().HttpContext.User);
        }


        public static void AddNakedFramework(this IServiceCollection services, Action<NakedCoreOptions> setupAction) {
            var options = new NakedCoreOptions(services);
            setupAction(options);

            services.AddNakedCoreFramework(options);
        }
    }
}