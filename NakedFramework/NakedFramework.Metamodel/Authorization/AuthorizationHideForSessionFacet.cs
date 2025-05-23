// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.Facet;

namespace NakedFramework.Metamodel.Authorization;

[Serializable]
public sealed class AuthorizationHideForSessionFacet : HideForSessionFacetAbstract {
    private readonly IIdentifier identifier;

    public AuthorizationHideForSessionFacet(IIdentifier identifier) => this.identifier = identifier;

    public override string HiddenReason(INakedObjectAdapter target, INakedFramework framework) {
        if (framework.ServiceProvider.GetService<IAuthorizationManager>() is { } authorizationManager) {
            return authorizationManager.IsVisible(framework, target, identifier)
                ? null
                : "Not authorized to view";
        }

        throw new NakedObjectSystemException($"Attempting Authorization on {identifier} but missing AuthorizationManager");
    }
}