﻿// // Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// // Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// // Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using Microsoft.Extensions.Configuration;

namespace NakedObjects.DependencyInjection.Extensions {
    public class EntityPersistorOptions {
        public MergeOption DefaultMergeOption { get; set; } = MergeOption.AppendOnly;
        public bool EnforceProxies { get; set; }  = true;
        public Func<bool> IsInitializedCheck { get; set; } = () => true;
        public int MaximumCommitCycles { get; set; } = 10;
        public Func<Type[]> NotPersistedTypes { get; set; }  = () => new Type[] { };
        public bool RollBackOnError { get; set; } = false;
        public Func<IConfiguration, DbContext>[] ContextInstallers { get; set; }
        public Action<ObjectContext> CustomConfig { get; set; } = oc => { };
        public bool RequireExplicitAssociationOfTypes { get; set; } = false;
    }
}