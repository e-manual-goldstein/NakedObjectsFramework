// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedObjects.Reflector.FacetFactory;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NakedObjects.Reflector.Test.FacetFactory;

[TestClass]
public class NotPersistedAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
    private NotPersistedAnnotationFacetFactory facetFactory;

    protected override Type[] SupportedTypes => new[] { typeof(INotPersistedFacet) };

    protected override IFacetFactory FacetFactory => facetFactory;

    [TestMethod]
    public override void TestFeatureTypes() {
        var featureTypes = facetFactory.FeatureTypes;
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Properties));
        Assert.IsTrue(featureTypes.HasFlag(FeatureType.Collections));
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.Actions));
        Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
    }

    [TestMethod]
    public void TestNotPersistedAnnotationPickedUpOnCollection() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer1), "Orders");
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(INotPersistedFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is NotPersistedFacet);
        AssertNoMethodsRemoved();
        Assert.IsNotNull(metamodel);
    }

    [TestMethod]
    public void TestNotPersistedAnnotationPickedUpOnProperty() {
        IImmutableDictionary<string, ITypeSpecBuilder> metamodel = new Dictionary<string, ITypeSpecBuilder>().ToImmutableDictionary();

        var property = FindProperty(typeof(Customer), "FirstName");
        metamodel = facetFactory.Process(Reflector, property, MethodRemover, Specification, metamodel);
        var facet = Specification.GetFacet(typeof(INotPersistedFacet));
        Assert.IsNotNull(facet);
        Assert.IsTrue(facet is NotPersistedFacet);
        AssertNoMethodsRemoved();
        Assert.IsNotNull(metamodel);
    }

    #region Nested type: Customer

    private class Customer {
#pragma warning disable CS0618 // Type or member is obsolete
        [NotPersisted]
#pragma warning restore CS0618 // Type or member is obsolete
// ReSharper disable once UnusedMember.Local
        public string FirstName => null;
    }

    #endregion

    #region Nested type: Customer1

    private class Customer1 {
#pragma warning disable CS0618 // Type or member is obsolete
        [NotPersisted]
#pragma warning restore CS0618 // Type or member is obsolete
// ReSharper disable once UnusedMember.Local
        public IList Orders => null;
    }

    #endregion

    #region Setup/Teardown

    [TestInitialize]
    public override void SetUp() {
        base.SetUp();
        facetFactory = new NotPersistedAnnotationFacetFactory(GetOrder<NotPersistedAnnotationFacetFactory>(), LoggerFactory);
    }

    [TestCleanup]
    public override void TearDown() {
        facetFactory = null;
        base.TearDown();
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.