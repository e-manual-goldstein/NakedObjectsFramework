﻿using System.Net.Http.Headers;
using System.Security.Principal;
using Microsoft.Extensions.Configuration.Memory;
using NakedFramework.Architecture.Component;
using NakedFramework.Metamodel.SpecImmutable;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.API;
using ROSI.Apis;
using ROSI.Records;

namespace NakedFramework.RATL.TestCase;

public abstract class BaseRATLMSTestTestCase {
    private static IHost? host;

    protected static IServiceProvider? RootServiceProvider;
    private IServiceProvider? scopeServiceProvider;

    protected virtual IServiceScope? ServiceScope { set; get; }

    protected static IPrincipal TestPrincipal => CreatePrincipal("Test", Array.Empty<string>());

    private static IHostBuilder CreateHostBuilder(string[] args, Action<IServiceCollection> configureServices, Func<IDictionary<string, string>> configuration) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults((a) => { })
            .ConfigureAppConfiguration((hostContext, configBuilder) => {
                var config = new MemoryConfigurationSource {
                    InitialData = configuration()
                };
                configBuilder.Add(config);
            })
            .ConfigureServices((hostContext, services) => configureServices(services));

    protected virtual void StartTest() {
        ServiceScope = RootServiceProvider.CreateScope();
        scopeServiceProvider = ServiceScope.ServiceProvider;
    }

    protected virtual void EndTest() {
        ServiceScope?.Dispose();
        ServiceScope = null;
        scopeServiceProvider = null;
    }

    protected static void InitializeNakedObjectsFramework(Action<IServiceCollection> configureServices, Func<IDictionary<string, string>>? configuration = null) {
        host = CreateHostBuilder(Array.Empty<string>(), configureServices, configuration ?? (() => new Dictionary<string, string>())).Build();
        RootServiceProvider = host.Services;
        RootServiceProvider.GetService<IModelBuilder>().Build();
    }

    protected static void CleanupNakedObjectsFramework() {
        ImmutableSpecFactory.ClearCache();
        RootServiceProvider.GetService<ISpecificationCache>().Clear();

        RootServiceProvider = null;
        host.StopAsync().GetAwaiter().GetResult();
        host.Dispose();
    }

    public RestfulObjectsControllerBase Api() {
        var sp = scopeServiceProvider;
        var api = sp.GetService<RestfulObjectsController>();
        return TestHelpers.SetMockContext(api, sp);
    }

    protected static IPrincipal CreatePrincipal(string name, string[] roles) => new GenericPrincipal(new GenericIdentity(name), roles);

    public DomainObject GetObject(string type, string id) => ROSIApi.GetObject(new Uri("http://localhost/"), type, id, TestInvokeOptions()).Result;

    public TestInvokeOptions TestInvokeOptions(string? token = null, EntityTagHeaderValue? tag = null) =>
        new(Api) {
            Token = token,
            Tag = tag
        };
}

// Copyright (c) Naked Objects Group Ltd.