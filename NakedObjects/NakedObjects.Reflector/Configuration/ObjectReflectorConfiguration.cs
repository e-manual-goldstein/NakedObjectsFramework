﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Core;
using NakedObjects.Core.Configuration;

namespace NakedObjects.Reflector.Configuration {
    [Serializable]
    public class ObjectReflectorConfiguration : IObjectReflectorConfiguration {
        public ObjectReflectorConfiguration(Type[] typesToIntrospect,
                                            Type[] services,
                                            bool concurrencyChecking = true) {
            
            TypesToIntrospect = typesToIntrospect;
            Services = services;
            IgnoreCase = false;
            ConcurrencyChecking = concurrencyChecking;

            ValidateConfig();
        }

        // for testing
        public static bool NoValidate { get; set; }

        private void ValidateConfig() {
            if (NoValidate) {
                return;
            }

            var msg = "Reflector configuration errors;\r\n";
            var configError = false;

            if (Services == null || !Services.Any()) {
                configError = true;
                msg += "No services specified;\r\n";
            }

            if (configError) {
                throw new InitialisationException(msg);
            }
        }

        private static Type EnsureGenericTypeIsComplete(Type type) {
            if (type.IsGenericType && !type.IsConstructedGenericType) {
                var genericType = type.GetGenericTypeDefinition();
                var genericParms = genericType.GetGenericArguments().Select(a => typeof(object)).ToArray();

                return type.GetGenericTypeDefinition().MakeGenericType(genericParms);
            }

            return type;
        }

        private Type[] GetObjectTypesToIntrospect() {
            var types = TypesToIntrospect.Select(EnsureGenericTypeIsComplete);
            return types.ToArray();
        }

        #region IObjectReflectorConfiguration Members

        public Type[] TypesToIntrospect { get; }
        public bool IgnoreCase { get; }
        public bool ConcurrencyChecking { get; }
        public bool HasConfig() => TypesToIntrospect.Any() && Services.Any();

        public Type[] Services { get; }

        public Type[] ObjectTypes => Services.Union(GetObjectTypesToIntrospect()).ToArray();

        #endregion
    }
}