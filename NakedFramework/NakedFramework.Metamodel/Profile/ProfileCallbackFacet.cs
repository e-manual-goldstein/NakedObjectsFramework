// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Profile;

namespace NakedFramework.Metamodel.Profile;

[Serializable]
public sealed class ProfileCallbackFacet : CallbackFacetAbstract,
                                           ICreatedCallbackFacet,
                                           IDeletedCallbackFacet,
                                           IDeletingCallbackFacet,
                                           ILoadedCallbackFacet,
                                           ILoadingCallbackFacet,
                                           IPersistedCallbackFacet,
                                           IPersistingCallbackFacet,
                                           IUpdatedCallbackFacet,
                                           IUpdatingCallbackFacet {
    private readonly ProfileEvent associatedEvent;

    private readonly ICallbackFacet underlyingFacet;

    public ProfileCallbackFacet(ProfileEvent associatedEvent, ICallbackFacet underlyingFacet) {
        this.associatedEvent = associatedEvent;
        this.underlyingFacet = underlyingFacet;
    }

    public override Type FacetType => underlyingFacet.FacetType;

    #region ICreatedCallbackFacet Members

    public override void Invoke(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
        var profileManager = framework.ServiceProvider.GetService<IProfileManager>();
        profileManager?.Begin(framework.Session, associatedEvent, "", nakedObjectAdapter, framework.LifecycleManager);
        try {
            underlyingFacet.Invoke(nakedObjectAdapter, framework);
        }
        finally {
            profileManager?.End(framework.Session, associatedEvent, "", nakedObjectAdapter, framework.LifecycleManager);
        }
    }

    #endregion

    public bool IsActive => true;
}