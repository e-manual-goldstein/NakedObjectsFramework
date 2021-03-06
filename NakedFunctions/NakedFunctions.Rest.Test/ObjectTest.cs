// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NakedFunctions.Reflector.Configuration;
using NakedFunctions.Rest.Test.Data;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Core.Configuration;
using NakedObjects.Facade;
using NakedObjects.Facade.Impl;
using NakedObjects.Facade.Impl.Implementation;
using NakedObjects.Facade.Impl.Utility;
using NakedObjects.Facade.Interface;
using NakedObjects.Facade.Translation;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Reflector.Configuration;
using NakedObjects.Rest;
using NakedObjects.Rest.Model;
using NakedObjects.Xat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace NakedFunctions.Rest.Test {

    public class NullStringHasher : IStringHasher {
        public string GetHash(string toHash) => null;
    }


    public class ObjectTest : AcceptanceTestCase {
        protected override Type[] Functions  {get; } = {typeof(SimpleRecordFunctions)};

        protected override Type[] Records { get; } = { typeof(SimpleRecord), typeof(GuidRecord), typeof(IAlert) };

        protected override Type[] ObjectTypes { get; } = { };

        protected override Type[] Services { get; } = { };

        protected override bool EnforceProxies => false;

        protected override Func<IConfiguration, DbContext>[] ContextInstallers =>
            new Func<IConfiguration, DbContext>[] { config => new ObjectDbContext()};


        protected override void RegisterTypes(IServiceCollection services) {
            base.RegisterTypes(services);
            services.AddTransient<RestfulObjectsController, RestfulObjectsController>();
            services.AddMvc(options => options.EnableEndpointRouting = false)
                    .AddNewtonsoftJson(options => options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc);
        }


        [SetUp]
        public void SetUp() => StartTest();

        [TearDown]
        public void TearDown() => EndTest();

        [OneTimeSetUp]
        public void FixtureSetUp() {
            ObjectReflectorConfiguration.NoValidate = true;
            InitializeNakedObjectsFramework(this);
        }

        [OneTimeTearDown]
        public void FixtureTearDown() {
            CleanupNakedObjectsFramework(this);
            ObjectDbContext.Delete();
        }

        protected RestfulObjectsControllerBase Api() {
            var sp = GetConfiguredContainer();
            var api = sp.GetService<RestfulObjectsController>();
            return Helpers.SetMockContext(api, sp);
        }

        private JObject GetObject(string type, string id) {
            var api = Api().AsGet();
            var result = api.GetObject(type, id);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            return JObject.Parse(json);
        }

        [Test]
        public void TestASimpleRecord() {
            var parsedResult = GetObject($"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");

            Assert.AreEqual("Fred", parsedResult["title"].ToString());

            var members = parsedResult["members"] as JObject;
            Assert.AreEqual(7, members?.Count);

            var function = members[nameof(SimpleRecordFunctions.ReShowRecord)];

            function.AssertAction(nameof(SimpleRecordFunctions.ReShowRecord), "{}");

            function["extensions"].AssertExtensions(5); // todo add 

            var propertyId = members[nameof(SimpleRecord.Id)];

            propertyId.AssertProperty(nameof(SimpleRecord.Id), "1", false);
            propertyId["extensions"].AssertExtensions(6); // todo add 

            var propertyName = members[nameof(SimpleRecord.Name)];

            propertyName.AssertProperty(nameof(SimpleRecord.Name), "Fred", false);
            propertyName["extensions"].AssertExtensions(8); // todo add 

            var links = function["links"] as JArray;

            Assert.AreEqual(2, links.Count);

            var invokeLink = links.Last;
            invokeLink.AssertObjectInvokeLink("{}", "GET", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1", nameof(SimpleRecordFunctions.ReShowRecord));
        }


        [Test]
        public void TestInvokeReShowRecord()
        {
            var api = Api();
            var result = api.GetInvoke($"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1", nameof(SimpleRecordFunctions.ReShowRecord), new ArgumentMap { Map = new Dictionary<string, IValue>() });
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);
            
            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
        }

        [Test]
        public void TestInvokeUpdateSimpleRecord()
        {
            var api = Api().AsPost();
            var map = new ArgumentMap {Map = new Dictionary<string, IValue>() {{"name", new ScalarValue("Fred3")}}};

            var result = api.PostInvoke($"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1", nameof(SimpleRecordFunctions.UpdateSimpleRecord), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred3", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
            Assert.AreEqual("Fred3", resultObj["members"]["Name"]["value"].ToString());
        }

        [Test]
        public void TestInvokeUpdateAndPersistSimpleRecord()
        {
            var api = Api().AsPost();
            var map = new ArgumentMap { Map = new Dictionary<string, IValue>() { { "name", new ScalarValue("Fred4") } } };

            var result = api.PostInvoke($"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1", nameof(SimpleRecordFunctions.UpdateAndPersistSimpleRecord), map);
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred4", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
            Assert.AreEqual("Fred4", resultObj["members"]["Name"]["value"].ToString());
        }

        [Test]
        public void TestInvokeWithWarning()
        {
            var api = Api();
            var result = api.GetInvoke($"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1", nameof(SimpleRecordFunctions.GetSimpleRecordWithWarning), new ArgumentMap { Map = new Dictionary<string, IValue>() });
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("object", parsedResult["resultType"].ToString());

            Assert.AreEqual("a warning", parsedResult["extensions"]["x-ro-nof-warnings"][0].ToString());

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred4", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");
        }

        [Test]
        public void TestInvokeWithLog()
        {
            var api = Api();
            var result = api.GetInvoke($"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1", nameof(SimpleRecordFunctions.GetSimpleRecordWithLog), new ArgumentMap { Map = new Dictionary<string, IValue>() });
            var (json, sc, _) = Helpers.ReadActionResult(result, api.ControllerContext.HttpContext);

            // no check of log explicitly - if logger not setup will throw exception and return 500
            Assert.AreEqual((int)HttpStatusCode.OK, sc);
            var parsedResult = JObject.Parse(json);

            Assert.AreEqual("object", parsedResult["resultType"].ToString());

            var resultObj = parsedResult["result"];

            resultObj.AssertObject("Fred4", $"NakedFunctions.Rest.Test.Data.{nameof(SimpleRecord)}", "1");

           
        }

        [Test]
        public void TestARecordWithGuid()
        {
            var parsedResult = GetObject($"NakedFunctions.Rest.Test.Data.{nameof(GuidRecord)}", "1");

            Assert.AreEqual("00000001-0002-0003-0405-060708090a0b", parsedResult["title"].ToString());

            var members = parsedResult["members"] as JObject;
            Assert.AreEqual(2, members?.Count);


            var propertyName = members[nameof(GuidRecord.Name)];

            propertyName.AssertProperty(nameof(GuidRecord.Name), "00000001-0002-0003-0405-060708090a0b", false);
            propertyName["extensions"].AssertExtensions(5); // todo add 

        }
    }
}