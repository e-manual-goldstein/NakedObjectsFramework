﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFunctions.Meta.Facet;
using NakedObjects;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Util;

namespace NakedFunctions.Meta.Test.Facet {
    [TestClass]
    public class InjectedQueryableParameterFacetTest {
        private readonly Mock<INakedObjectsFramework> mockFramework = new Mock<INakedObjectsFramework>();
        private readonly Mock<IObjectPersistor> mockPersistor = new Mock<IObjectPersistor>();

        private readonly IQueryable<object> TestValue = new QueryableList<object>();

        public InjectedQueryableParameterFacetTest() {
            mockFramework.SetupGet(p => p.Persistor).Returns(mockPersistor.Object);
            mockPersistor.Setup(p => p.UntrackedInstances<object>()).Returns(TestValue);
        }


        [TestMethod]
        public void TestInject() {
            var testFacet = new InjectedQueryableParameterFacet(null, typeof(object));

            var result = testFacet.GetInjectedValue(mockFramework.Object);

            Assert.AreEqual(result, TestValue);
        }
    }
}