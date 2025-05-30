﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Interactions;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.Facet;

namespace NakedFramework.Metamodel.Test.Facet;

[TestClass]
public class DisabledFacetTest {
    [TestMethod]
    public void TestDisabledFacetAlways() {
        IDisabledFacet facet = DisabledFacetAlways.Instance;
        Assert.AreEqual(NakedObjects.Resources.NakedObjects.AlwaysDisabled, facet.DisabledReason(null));
    }

    [TestMethod]
    public void TestDisabledFacetAnnotationAlways() {
        IDisabledFacet facet = new DisabledFacetAnnotation(WhenTo.Always);
        Assert.AreEqual(NakedObjects.Resources.NakedObjects.AlwaysDisabled, facet.DisabledReason(null));
    }

    [TestMethod]
    public void TestDisabledFacetAnnotationNever() {
        IDisabledFacet facet = new DisabledFacetAnnotation(WhenTo.Never);
        Assert.IsNull(facet.DisabledReason(null));
    }

    private static INakedObjectAdapter Mock(object obj) {
        var mockParm = new Mock<INakedObjectAdapter>();
        mockParm.Setup(p => p.Object).Returns(obj);
        return mockParm.Object;
    }

    [TestMethod]
    public void TestDisabledFacetAnnotationAsInteraction() {
        IDisablingInteractionAdvisor facet = new DisabledFacetAnnotation(WhenTo.Never);
        var target = Mock(new object());
        var mockIc = new Mock<IInteractionContext>();
        mockIc.Setup(ic => ic.Target).Returns(target);

        var e = facet.CreateExceptionFor(mockIc.Object);

        Assert.IsInstanceOfType(e, typeof(DisabledException));
        Assert.AreEqual("Exception of type 'NakedFramework.Core.Error.DisabledException' was thrown.", e.Message);
    }
}