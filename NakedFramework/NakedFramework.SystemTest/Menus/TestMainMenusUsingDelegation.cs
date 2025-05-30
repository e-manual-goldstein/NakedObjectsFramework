﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NakedFramework.Architecture.Framework;
using NakedFramework.DependencyInjection.Extensions;
using NakedFramework.Menu;
using NakedFramework.Persistor.EF6.Extensions;
using NakedFramework.RATL.Classic.TestCase;
using NakedFramework.RATL.Helpers;
using NakedFramework.Rest.Extensions;
using NakedFramework.Test.Interface;
using NakedFramework.Test.TestObjects;
using NakedObjects;
using NakedObjects.Reflector.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using TestObjectMenu;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedFramework.SystemTest.Menus;

[TestFixture]
public class TestMainMenusUsingDelegation : AcceptanceTestCase {
    [SetUp]
    public void SetUp() {
        StartTest();
        NakedFramework = ServiceScope.ServiceProvider.GetService<INakedFramework>();
    }

    [TearDown]
    public void TearDown() => EndTest();

    [OneTimeSetUp]
    public void FixtureSetUp() {
        InitializeNakedObjectsFramework(this);
    }

    [OneTimeTearDown]
    public void FixtureTearDown() {
        CleanupNakedObjectsFramework(this);
    }

    protected override void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services) {
        services.AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
        services.AddMvc(options => options.EnableEndpointRouting = false);
        services.AddHttpContextAccessor();
        services.AddNakedFramework(frameworkOptions => {
            frameworkOptions.AddEF6Persistor(options => { options.ContextCreators = ContextCreators; });
            frameworkOptions.AddRestfulObjects(restOptions => { });
            frameworkOptions.MainMenus = MainMenus;
            frameworkOptions.AddNakedObjects(appOptions => {
                appOptions.DomainModelTypes = Array.Empty<Type>();
                appOptions.DomainModelServices = Services;
            });
        });
        services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
        services.AddScoped(p => TestPrincipal);
        var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
        services.AddSingleton<DiagnosticListener>(diagnosticSource);
        services.AddSingleton<DiagnosticSource>(diagnosticSource);
    }

    protected INakedFramework NakedFramework { get; set; }

    protected Func<IConfiguration, DbContext>[] ContextCreators =>
        new Func<IConfiguration, DbContext>[] { config => new MenusDbContext() };

    protected  Type[] Services =>
        new[] {
            typeof(FooService),
            typeof(ServiceWithSubMenus),
            typeof(BarService)
        };

    protected  IMenu[] MainMenus(IMenuFactory factory) => LocalMainMenus.MainMenus(factory);

    [Test]
    public virtual void TestMainMenus() {
        var menus = AllMainMenus();

        menus[0].AssertNameEquals("Foo Service");
        menus[1].AssertNameEquals("Bars"); //Picks up Named attribute on service
        menus[2].AssertNameEquals("Subs"); //Named attribute overridden in menu construction

        var foo = menus[0];
        foo.AssertItemCountIs(3);
        Assert.AreEqual(3, foo.AllItems().Count(i => i != null));

        foo.AllItems()[0].AssertNameEquals("Foo Action0");
        foo.AllItems()[1].AssertNameEquals("Foo Action1");
        foo.AllItems()[2].AssertNameEquals("Foo Action2");
    }
}

#region Classes used in test

public class LocalMainMenus {
    public static IMenu[] MainMenus(IMenuFactory factory) {
        var menuDefs = new Dictionary<Type, Action<IMenu>> {
            { typeof(FooService), FooService.Menu },
            { typeof(BarService), BarService.Menu },
            { typeof(ServiceWithSubMenus), ServiceWithSubMenus.Menu }
        };

        var menus = new List<IMenu>();
        foreach (var menuDef in menuDefs) {
            var menu = factory.NewMenu(menuDef.Key);
            menuDef.Value(menu);
            menus.Add(menu);
        }

        return menus.ToArray();
    }
}

public class FooService {
    public static void Menu(IMenu menu) {
        menu.AddRemainingActions(typeof(FooService));
    }

    public void FooAction0() { }

    public void FooAction1() { }

    public void FooAction2(string p1, int p2) { }
}

[Named("Subs")]
public class ServiceWithSubMenus {
    public static void Menu(IMenu menu) {
        var type = typeof(ServiceWithSubMenus);
        var sub1 = menu.CreateSubMenu("Sub1");
        sub1.AddAction(type, "Action1");
        sub1.AddAction(type, "Action3");
        var sub2 = menu.CreateSubMenu("Sub2");
        sub2.AddAction(type, "Action2");
        sub2.AddAction(type, "Action0");
    }

    public void Action0() { }

    public void Action1() { }

    public void Action2() { }

    public void Action3() { }
}

[Named("Bars")]
public class BarService {
    public static void Menu(IMenu menu) {
        menu.AddRemainingActions(typeof(BarService));
    }

    [MemberOrder(10)]
    public void BarAction0() { }

    [MemberOrder(1)]
    public void BarAction1() { }

    public void BarAction2() { }

    public void BarAction3() { }
}

#endregion