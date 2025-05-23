// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Metamodel.Audit;
using NakedFramework.Metamodel.Authorization;
using NakedFramework.Metamodel.Profile;
using NakedFramework.Metamodel.SpecImmutable;
using NakedFramework.Persistor.EF6.Extensions;
using NakedFramework.Rest.Extensions;
using NakedFramework.Test.Interface;
using NakedFramework.Test.TestObjects;
using NakedFunctions.Reflector.Extensions;
using NakedObjects.Reflector.Extensions;

namespace NakedFramework.Test.TestCase;

//[Obsolete("#498")]
public abstract class AcceptanceTestCase {
    private static IHost host;

    protected IServiceProvider RootServiceProvider;
    private IServiceProvider scopeServiceProvider;
    private IDictionary<string, ITestService> servicesCache = new Dictionary<string, ITestService>();
    private ITestObjectFactory testObjectFactory;
    private IPrincipal testPrincipal;

    private AcceptanceTestCase(string name) {
        DbProviderFactories.RegisterFactory("Microsoft.Data.SqlClient", SqlClientFactory.Instance);
    }

    protected AcceptanceTestCase() : this("Unnamed") { }

    protected virtual IServiceScope ServiceScope { set; get; }

    protected virtual ITestObjectFactory TestObjectFactoryClass => testObjectFactory ??= new TestObjectFactory(NakedFramework);

    protected virtual IPrincipal TestPrincipal {
        get { return testPrincipal ??= CreatePrincipal("Test", Array.Empty<string>()); }
    }

    protected INakedFramework NakedFramework { get; private set; }

    protected ILoggerFactory LoggerFactory { get; private set; }

    protected virtual object[] Fixtures => Array.Empty<object>();

    /// <summary>
    ///     By default this returns the union of the types specified in MenuServices, OrderedContributedActions
    ///     & SystemServices. This is for backwards compatibility only.
    ///     The property may be overridden to return a fresh list of types, in which case Menu Services etc
    ///     will be ignored.
    /// </summary>
    protected virtual Type[] Services => Array.Empty<Type>();

    protected virtual Type[] ObjectTypes => Array.Empty<Type>();

    protected virtual Type[] Records => Array.Empty<Type>();

    protected virtual Type[] Functions => Array.Empty<Type>();

    protected virtual Func<Type[], Type[]> SupportedSystemTypes => t => t;

    protected virtual Func<Type[]> NotPersistedTypes => Array.Empty<Type>;

    protected virtual Func<IConfiguration, DbContext>[] ContextCreators => null;

    protected virtual IAuthorizationConfiguration AuthorizationConfiguration => null;

    protected virtual IAuditConfiguration AuditConfiguration => null;

    protected virtual IProfileConfiguration ProfileConfiguration => null;

    protected virtual bool EnforceProxies => true;

    protected virtual Action<EF6PersistorOptions> PersistorOptions =>
        options => {
            options.ContextCreators = ContextCreators;
            options.EnforceProxies = EnforceProxies;
            options.NotPersistedTypes = NotPersistedTypes;
        };

    protected virtual Action<NakedObjectsOptions> NakedObjectsOptions =>
        options => {
            options.DomainModelTypes = ObjectTypes;
            options.DomainModelServices = Services;
            options.NoValidate = true;
        };

    protected virtual Action<NakedFunctionsOptions> NakedFunctionsOptions =>
        options => {
            options.DomainTypes = Records;
            options.DomainFunctions = Functions;
            options.DomainServices = Services;
        };

    protected virtual Action<RestfulObjectsOptions> RestfulObjectsOptions => options => { options.CacheSettings = (0, 3600, 86400); };

    protected virtual Action<NakedFrameworkOptions> NakedFrameworkOptions =>
        builder => {
            AddCoreOptions(builder);
            AddPersistor(builder);
            AddNakedObjects(builder);
            AddNakedFunctions(builder);
            AddRestfulObjects(builder);
        };

    protected virtual Action<NakedFrameworkOptions> AddCoreOptions => builder => {
        builder.MainMenus = MainMenus;
        builder.AuthorizationConfiguration = AuthorizationConfiguration;
        builder.AuditConfiguration = AuditConfiguration;
        builder.ProfileConfiguration = ProfileConfiguration;
        builder.SupportedSystemTypes = SupportedSystemTypes;
    };

    protected virtual Action<NakedFrameworkOptions> AddPersistor => builder => builder.AddEF6Persistor(PersistorOptions);

    protected virtual Action<NakedFrameworkOptions> AddNakedObjects => builder => builder.AddNakedObjects(NakedObjectsOptions);

    protected virtual Action<NakedFrameworkOptions> AddNakedFunctions => builder => builder.AddNakedFunctions(NakedFunctionsOptions);

    protected virtual Action<NakedFrameworkOptions> AddRestfulObjects => builder => builder.AddRestfulObjects(RestfulObjectsOptions);

    private IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults((a) => { })
            .ConfigureAppConfiguration((hostContext, configBuilder) => {
                var config = new MemoryConfigurationSource {
                    InitialData = Configuration()
                };
                configBuilder.Add(config);
            })
            .ConfigureServices((hostContext, services) => { RegisterTypes(services); });

    // used from F# test code do not delete
    // ReSharper disable once UnusedMember.Global
    protected virtual IServiceProvider GetConfiguredContainer() => scopeServiceProvider;

    protected virtual IMenu[] MainMenus(IMenuFactory factory) => null; //Allows tests not to define menus if not needed.

    protected virtual void StartTest() {
        ServiceScope = RootServiceProvider.CreateScope();
        scopeServiceProvider = ServiceScope.ServiceProvider;
        NakedFramework = scopeServiceProvider.GetService<INakedFramework>();
        LoggerFactory = scopeServiceProvider.GetService<ILoggerFactory>();
    }

    protected virtual void EndTest() {
        ServiceScope?.Dispose();
        ServiceScope = null;
        scopeServiceProvider = null;
        servicesCache = new Dictionary<string, ITestService>();
        testObjectFactory = null;
        testPrincipal = null;
    }

    private void InstallFixtures(ITransactionManager transactionManager, IDomainObjectInjector injector, object[] newFixtures) {
        foreach (var fixture in newFixtures) {
            InstallFixture(transactionManager, injector, fixture);
        }
    }

    private static void SetValue(PropertyInfo property, object injectee, object value) {
        if (property != null) {
            try {
                property.SetValue(injectee, value, null);
            }
            catch (TargetInvocationException e) {
                InvokeUtils.InvocationException("Exception executing " + property, e);
            }
        }
    }

    private static MethodInfo GetInstallMethod(object fixture) =>
        fixture.GetType().GetMethod("Install", Array.Empty<Type>()) ??
        fixture.GetType().GetMethod("install", Array.Empty<Type>());

    protected virtual object[] GetFixtures(object fixture) {
        var getFixturesMethod = fixture.GetType().GetMethod("GetFixtures", Array.Empty<Type>());
        return getFixturesMethod == null ? Array.Empty<object>() : (object[])getFixturesMethod.Invoke(fixture, Array.Empty<object>());
    }

    protected virtual void InstallFixture(object fixture) {
        var property = fixture.GetType().GetProperty("Service");
        SetValue(property, fixture, null);

        var installMethod = GetInstallMethod(fixture);
        try {
            installMethod.Invoke(fixture, Array.Empty<object>());
        }
        catch (TargetInvocationException e) {
            InvokeUtils.InvocationException("Exception executing " + installMethod, e);
        }
    }

    private void InstallFixture(ITransactionManager transactionManager, IDomainObjectInjector injector, object fixture) {
        injector.InjectInto(fixture);

        // first, install any child fixtures (if this is a composite.
        var childFixtures = GetFixtures(fixture);
        InstallFixtures(transactionManager, injector, childFixtures);

        // now, install the fixture itself
        try {
            transactionManager.StartTransaction();
            InstallFixture(fixture);
            transactionManager.EndTransaction();
        }
        catch (Exception e) {
            var msg = $"installing fixture {fixture.GetType().FullName} failed ({e.Message}); aborting fixture ";
            try {
                transactionManager.AbortTransaction();
            }
            catch (Exception e2) {
                msg = $"{msg} - {e2.Message} failure during abort";
            }

            throw new NakedObjectSystemException(msg, e);
        }
    }

    protected virtual void RunFixtures() {
        using var fixtureServiceScope = RootServiceProvider.CreateScope();
        NakedFramework = fixtureServiceScope.ServiceProvider.GetService<INakedFramework>();
        InstallFixtures(NakedFramework.TransactionManager, NakedFramework.DomainObjectInjector, Fixtures);
        NakedFramework = null;
    }

    protected ITestService GetTestService<T>() => GetTestService(typeof(T));

    protected virtual ITestService GetTestService(Type type) {
        var testService = NakedFramework.ServicesManager.GetServices().Where(no => type.IsInstanceOfType(no.Object)).Select(no => TestObjectFactoryClass.CreateTestService(no.Object)).FirstOrDefault();
        if (testService == null) {
            Assert.Fail("No service of type " + type);
        }

        return testService;
    }

    protected virtual ITestService GetTestService(string serviceName) {
        if (!servicesCache.ContainsKey(serviceName.ToLower())) {
            foreach (var service in NakedFramework.ServicesManager.GetServices()) {
                if (service.TitleString().Equals(serviceName, StringComparison.CurrentCultureIgnoreCase)) {
                    var testService = TestObjectFactoryClass.CreateTestService(service.Object);
                    if (testService == null) {
                        Assert.Fail("Invalid service name " + serviceName);
                    }

                    servicesCache[serviceName.ToLower()] = testService;
                    return testService;
                }
            }

            Assert.Fail("No such service: " + serviceName);
            return null;
        }

        return servicesCache[serviceName.ToLower()];
    }

    protected virtual ITestMenu GetMainMenu(string menuName) {
        var mainMenus = NakedFramework.MetamodelManager.MainMenus();
        if (mainMenus.Any()) {
            var menu = mainMenus.FirstOrDefault(m => m.Name == menuName);
            if (menu == null) {
                Assert.Fail("No such main menu " + menuName);
            }

            return TestObjectFactoryClass.CreateTestMenuMain(menu);
        }

        //Use the MenuServices to derive the menus
        var service = GetTestService(menuName);
        if (service == null) {
            Assert.Fail("No such main menu, or Service, " + menuName);
        }

        return service.GetMenu();
    }

    protected virtual ITestMenu[] AllMainMenus() {
        return NakedFramework.MetamodelManager.MainMenus().Select(m => TestObjectFactoryClass.CreateTestMenuMain(m)).ToArray();
    }

    protected virtual void AssertMainMenuCountIs(int expected) {
        var actual = NakedFramework.MetamodelManager.MainMenus().Length;
        Assert.AreEqual(expected, actual);
    }

    private IPrincipal CreatePrincipal(string name, string[] roles) {
        return testPrincipal = new GenericPrincipal(new GenericIdentity(name), roles);
    }

    protected virtual void SetUser(string username, params string[] roles) {
        testPrincipal = CreatePrincipal(username, roles);
        var ts = NakedFramework?.Session as TestSession;
        ts?.ReplacePrincipal(testPrincipal);
    }

    protected virtual void SetUser(string username) {
        SetUser(username, Array.Empty<string>());
    }

    protected static void InitializeNakedObjectsFramework(AcceptanceTestCase tc) {
        host = tc.CreateHostBuilder(Array.Empty<string>()).Build();
        tc.RootServiceProvider = host.Services;
        tc.servicesCache = new Dictionary<string, ITestService>();
        tc.RootServiceProvider.GetService<IModelBuilder>().Build();
    }

    protected static void CleanupNakedObjectsFramework(AcceptanceTestCase tc) {
        ImmutableSpecFactory.ClearCache();
        tc.RootServiceProvider.GetService<ISpecificationCache>().Clear();
        tc.EndTest();
        tc.servicesCache.Clear();
        tc.servicesCache = null;
        tc.testObjectFactory = null;
        tc.RootServiceProvider = null;
        host.StopAsync().GetAwaiter().GetResult();
        host.Dispose();
    }

    protected virtual void RegisterTypes(IServiceCollection services) {
        services.AddNakedFramework(NakedFrameworkOptions);
        //Externals
        services.AddScoped(p => TestPrincipal);
        services.AddScoped<ISession, TestSession>();
        var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
        services.AddSingleton<DiagnosticListener>(diagnosticSource);
        services.AddSingleton<DiagnosticSource>(diagnosticSource);
    }

    protected virtual IDictionary<string, string> Configuration() =>
        new Dictionary<string, string>();
}

// Copyright (c) Naked Objects Group Ltd.