// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Resolve;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Core.Resolve;
using NakedFramework.Core.Util;
using static NakedFramework.Core.Util.ToStringHelpers;

namespace NakedFramework.Core.Adapter;

public sealed class NakedObjectAdapter : INakedObjectAdapter {
    private readonly INakedFramework framework;
    private readonly ILogger<NakedObjectAdapter> logger;
    private ITypeSpec spec;

    public NakedObjectAdapter(object domainObject,
                              IOid oid,
                              INakedFramework framework,
                              ILoggerFactory loggerFactory,
                              ILogger<NakedObjectAdapter> logger) {
        this.framework = framework;
        this.logger = logger ?? throw new InitialisationException($"{nameof(logger)} is null");

        if (domainObject is INakedObjectAdapter) {
            throw new AdapterException(logger.LogAndReturn($"Adapter can't be used to adapt an adapter: {domainObject}"));
        }

        Object = domainObject;
        Oid = oid;
        ResolveState = new ResolveStateMachine(this, framework.Session);
        Version = new NullVersion(loggerFactory.CreateLogger<NullVersion>());
    }

    private string DefaultTitle { get; set; }

    private ITypeOfFacet TypeOfFacet => Spec.GetFacet<ITypeOfFacet>();

    private ITypeSpec SetSpec() {
        spec = framework.MetamodelManager.GetSpecification(Object.GetType());
        DefaultTitle = "A" + (" " + spec.SingularName).ToLower();
        return spec;
    }

    private string CollectionTitleString(ICollectionFacet facet) {
        var size = CanCount() ? facet.AsEnumerable(this, framework.NakedObjectManager).Count() : CollectionUtils.IncompleteCollection;
        var elementSpecification = TypeOfFacet == null ? null : framework.MetamodelManager.GetSpecification(TypeOfFacet.GetValueSpec(this, framework.MetamodelManager.Metamodel));
        return CollectionUtils.CollectionTitleString(elementSpecification, size);
    }

    private bool CanCount() => !Spec.ContainsFacet<INotCountedFacet>();

    private string GetTitle() {
        // don't do title of unresolved objects as this may force the resolving of the object.
        if (ResolveState.IsTransient() || ResolveState.IsResolved() || ResolveState.IsAggregated()) {
            return $"title={TitleString()},";
        }

        return "";
    }

    public override string ToString() =>
        $"{NameAndHashCode(this)} [{AddProperties()}{GetTitle()}domainObject-hash=#{HashCode(Object)}]";

    private bool ShouldSetVersion(IVersion newVersion) => newVersion.IsDifferent(Version);

    private string AddProperties() {
        var objectType = Object.GetType();
        string GetOid() => Oid is not null ? $":{Oid}," : ":-,";

        string GetSpec() =>
            spec is null ? $"class={objectType.FullName}," : $"specification={spec.ShortName},Type={spec.FullName},";

        string GetProxy() =>
            FasterTypeUtils.IsAnyProxy(objectType) ? $"{objectType.FullName}," : "None,";

        string GetVersion() =>
            $"{Version?.AsSequence()},";

        return $"{ResolveState.CurrentState.Code}{GetOid()}{GetSpec()}proxy={GetProxy()}version={GetVersion()}";
    }

    private void CallCallback<T>() where T : ICallbackFacet => Spec.GetFacet<T>()?.Invoke(this, framework);

    #region INakedObjectAdapter Members

    public object Object { get; private set; }

    public IResolveStateMachine ResolveState { get; }

    public ITypeSpec Spec => spec ?? SetSpec();

    public IVersion Version { get; private set; }

    public IVersion OptimisticLock {
        set {
            if (ShouldSetVersion(value)) {
                Version = value;
            }
        }
    }

    /// <summary>
    ///     Returns the title from the underlying business object. If the object has not yet been resolved the
    ///     specification will be asked for a unresolved title, which could of been persisted by the persistence
    ///     mechanism. If either of the above provides null as the title then this method will return a title
    ///     relating to the name of the object type, e.g. "A Customer", "A Product".
    /// </summary>
    public string TitleString() {
        try {
            if (Spec.IsCollection && !Spec.IsParseable) {
                return CollectionTitleString(Spec.GetFacet<ICollectionFacet>());
            }

            return Spec.GetTitle(this) ?? DefaultTitle;
        }
        catch (Exception e) {
            throw new TitleException(logger.LogAndReturn("Exception on ToString POCO: " + (Object == null ? "unknown" : Object.GetType().FullName)), e);
        }
    }

    /// <summary>
    ///     Sometimes it is necessary to manage the replacement of the underlying domain object (by another
    ///     component such as an object store). This method allows the adapter to be kept while the domain object
    ///     is replaced.
    /// </summary>
    public void ReplacePoco(object obj) => Object = obj;

    public string ValidToPersist() {
        var objectSpec = Spec as IObjectSpec ?? throw new NakedObjectSystemException("Spec must be IObjectSpec to persist");
        var properties = objectSpec.Properties;
        foreach (var property in properties) {
            var referencedObjectAdapter = property.GetNakedObject(this);
            if (property.IsUsable(this).IsAllowed && property.IsVisible(this)) {
                if (property.IsMandatory && property.IsEmpty(this)) {
                    return string.Format(NakedObjects.Resources.NakedObjects.PropertyMandatory, objectSpec.ShortName, property.Name(this));
                }

                if (property is IOneToOneAssociationSpec associationSpec) {
                    var valid = associationSpec.IsAssociationValid(this, referencedObjectAdapter);
                    if (valid.IsVetoed) {
                        return string.Format(NakedObjects.Resources.NakedObjects.PropertyInvalid, objectSpec.ShortName, associationSpec.Name(this), valid.Reason);
                    }
                }
            }

            if (property is IOneToOneAssociationSpec) {
                if (referencedObjectAdapter is not null && referencedObjectAdapter.ResolveState.IsTransient()) {
                    var referencedObjectMessage = referencedObjectAdapter.ValidToPersist();
                    if (referencedObjectMessage is not null) {
                        return referencedObjectMessage;
                    }
                }
            }
        }

        return objectSpec.GetFacet<IValidateObjectFacet>()?.Validate(this, logger);
    }

    public void SetATransientOid(IOid newOid) {
        if (!newOid.IsTransient) {
            throw new NakedObjectSystemException("New Oid must be transient");
        }

        Oid = newOid;
    }

    public void CheckLock(IVersion otherVersion) {
        if (Version is not null && Version.IsDifferent(otherVersion)) {
            throw new ConcurrencyException(this);
        }
    }

    public IOid Oid { get; private set; }

    public void LoadAnyComplexTypes() {
        if (Spec is IServiceSpec ||
            Spec.IsViewModel ||
            Spec.ContainsFacet(typeof(IComplexTypeFacet))) {
            return;
        }

        framework.Persistor.LoadComplexTypes(this, ResolveState.IsGhost());
    }

    public void Created() => CallCallback<ICreatedCallbackFacet>();

    public void Deleting() => CallCallback<IDeletingCallbackFacet>();

    public void Deleted() => CallCallback<IDeletedCallbackFacet>();

    public void Loading() => CallCallback<ILoadingCallbackFacet>();

    public void Loaded() => CallCallback<ILoadedCallbackFacet>();

    public void Persisting() => CallCallback<IPersistingCallbackFacet>();

    public void Persisted() => CallCallback<IPersistedCallbackFacet>();

    public void Updating() => CallCallback<IUpdatingCallbackFacet>();

    public void Updated() => CallCallback<IUpdatedCallbackFacet>();

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.