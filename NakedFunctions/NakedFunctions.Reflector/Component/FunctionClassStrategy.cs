// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Configuration;
using NakedObjects.ParallelReflector.Component;

namespace NakedFunctions.Reflector.Component {
    /// <summary>
    ///     Standard way of determining which fields are to be exposed in a Naked Objects system.
    /// </summary>
    [Serializable]
    public class FunctionClassStrategy : AbstractClassStrategy {
        private readonly IFunctionalReflectorConfiguration config;

        public FunctionClassStrategy(IFunctionalReflectorConfiguration config) => this.config = config;

        protected override bool IsTypeIgnored(Type type) => type.GetCustomAttribute<NakedFunctionsIgnoreAttribute>() is not null;

        protected override bool IsTypeExplicitlyRequested(Type type) {
            var types = config.Types.Union(config.Functions).ToArray();
            return types.Any(t => t == type) ||
                   type.IsGenericType && types.Any(t => t == type.GetGenericTypeDefinition());
        }

        #region IClassStrategy Members

        public override bool IsIgnored(MemberInfo member) => member.GetCustomAttribute<NakedFunctionsIgnoreAttribute>() is not null;
        public override bool IsService(Type type) => false;
        public override bool LoadReturnType(MethodInfo method) => false;

        #endregion

        // because Sets don't implement IEnumerable<>
    }

    // Copyright (c) Naked Objects Group Ltd.
}