// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.ParallelReflect.Component;
using NakedObjects.Reflect.FacetFactory;
#pragma warning disable 612

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestClass]
    public class ProgramPersistableOnlyAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private ProgramPersistableOnlyAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof(IProgramPersistableOnlyFacet)}; }
        }

        protected override IFacetFactory FacetFactory => facetFactory;

        [TestMethod]
        public override void TestFeatureTypes() {
            var featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }

        [TestMethod]
        public void TestProgramPersistableOnlyNotPickup() {
            facetFactory.Process(Reflector, typeof(Customer1), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IProgramPersistableOnlyFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestProgramPersistableOnlyPickup() {
            facetFactory.Process(Reflector, typeof(Customer), MethodRemover, Specification);
            var facet = Specification.GetFacet(typeof(IProgramPersistableOnlyFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ProgramPersistableOnly);
            AssertNoMethodsRemoved();
        }

        #region Nested type: Customer

        [ProgramPersistableOnly]
        private class Customer { }

        #endregion

        #region Nested type: Customer1

        private class Customer1 { }

        #endregion

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ProgramPersistableOnlyAnnotationFacetFactory(new FacetFactoryOrder<ProgramPersistableOnlyAnnotationFacetFactory>(), LoggerFactory);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}