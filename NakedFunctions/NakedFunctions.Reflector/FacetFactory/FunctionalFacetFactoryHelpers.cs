﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using NakedObjects.Core.Util;

namespace NakedFunctions.Reflector.FacetFactory {
    public static class FunctionalFacetFactoryHelpers {

        public static bool IsInjectedParameter(MethodInfo method, int paramNum) {
            if (method.IsDefined(typeof(ExtensionAttribute), false) && paramNum == 0) {
                return false;
            }

            return method.GetParameters()[paramNum] switch {
                ParameterInfo parameter when parameter.IsDefined(typeof(InjectedAttribute), false) => true,
                ParameterInfo parameter when CollectionUtils.IsQueryable(parameter.ParameterType) => true,
                ParameterInfo parameter when parameter.ParameterType == typeof(Guid) => true,
                ParameterInfo parameter when parameter.ParameterType == typeof(IPrincipal) => true,
                _ => false
            };
        }

        public static Type GetContributedToType(MethodInfo method)
            => method.IsDefined(typeof(ExtensionAttribute), false) ? method.GetParameters().FirstOrDefault()?.ParameterType : null;
    }
}