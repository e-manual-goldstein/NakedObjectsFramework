// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Interactions;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Reflect {
    public abstract class MemberSpecAbstract : IMemberSpec {
        private readonly IMemberSpecImmutable memberSpecImmutable;

        protected internal MemberSpecAbstract(string id, IMemberSpecImmutable memberSpec, INakedObjectsFramework framework) {
            Id = id ?? throw new InitialisationException($"{nameof(id)} is null");
            memberSpecImmutable = memberSpec ?? throw new InitialisationException($"{nameof(memberSpec)} is null");
            Session = framework?.Session ?? throw new InitialisationException($"{nameof(framework.Session)} is null");
            LifecycleManager = framework.LifecycleManager ?? throw new InitialisationException($"{nameof(framework.LifecycleManager)} is null");
            MetamodelManager = framework.MetamodelManager ?? throw new InitialisationException($"{nameof(framework.MetamodelManager)} is null");
            Persistor = framework.Persistor ?? throw new InitialisationException($"{nameof(framework.Persistor)} is null"); ;
        }

        public ISession Session { get; }

        public IObjectPersistor Persistor { get; }

        public ILifecycleManager LifecycleManager { get; }

        protected IMetamodelManager MetamodelManager { get; }

        public abstract IObjectSpec ElementSpec { get; }

        #region IMemberSpec Members

        public virtual string Id { get; }

        public virtual IIdentifier Identifier => memberSpecImmutable.Identifier;

        public virtual Type[] FacetTypes => memberSpecImmutable.FacetTypes;

        /// <summary>
        ///     Return the default label for this member. This is based on the name of this member.
        /// </summary>
        /// <seealso cref="Id()" />
        public virtual string Name => memberSpecImmutable.Name;

        public virtual string Description => memberSpecImmutable.Description;

        public abstract IObjectSpec ReturnSpec { get; }

        public virtual bool ContainsFacet(Type facetType) => memberSpecImmutable.ContainsFacet(facetType);

        public virtual bool ContainsFacet<T>() where T : IFacet => memberSpecImmutable.ContainsFacet<T>();

        public virtual IFacet GetFacet(Type type) => memberSpecImmutable.GetFacet(type);

        public virtual T GetFacet<T>() where T : IFacet => memberSpecImmutable.GetFacet<T>();

        public virtual IEnumerable<IFacet> GetFacets() => memberSpecImmutable.GetFacets();

        /// <summary>
        ///     Loops over all <see cref="IHidingInteractionAdvisor" /> <see cref="IFacet" />s and
        ///     returns <c>true</c> only if none hide the member.
        /// </summary>
        public virtual bool IsVisible(INakedObjectAdapter target) {
            IInteractionContext ic = InteractionContext.AccessMember(Session, Persistor, false, target, Identifier);
            return InteractionUtils.IsVisible(this, ic, LifecycleManager, MetamodelManager);
        }

        public virtual bool IsVisibleWhenPersistent(INakedObjectAdapter target) {
            IInteractionContext ic = InteractionContext.AccessMember(Session, Persistor, false, target, Identifier);
            return InteractionUtils.IsVisibleWhenPersistent(this, ic, LifecycleManager, MetamodelManager);
        }

        /// <summary>
        ///     Loops over all <see cref="IDisablingInteractionAdvisor" /> <see cref="IFacet" />s and
        ///     returns <c>true</c> only if none disables the member.
        /// </summary>
        public virtual IConsent IsUsable(INakedObjectAdapter target) {
            IInteractionContext ic = InteractionContext
                .AccessMember(Session, Persistor, false, target, Identifier);
            return InteractionUtils.IsUsable(this, ic);
        }

        public bool IsNullable => memberSpecImmutable.ContainsFacet(typeof(INullableFacet));

        #endregion

        public override string ToString() => "id=" + Id + ",name='" + Name + "'";

        protected internal virtual IConsent GetConsent(string message) => message == null ? (IConsent) Allow.Default : new Veto(message);
    }

    // Copyright (c) Naked Objects Group Ltd.
}