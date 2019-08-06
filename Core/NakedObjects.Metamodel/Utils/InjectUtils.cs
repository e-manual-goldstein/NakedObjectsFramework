// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using NakedFunctions;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Utils {
    public static class InjectUtils {
        public static DateTime GetInjectedDateTimeValue() {
            return DateTime.Now;
        }

        public static Guid GetInjectedGuidValue() {
            return Guid.NewGuid();
        }

        public static int GetInjectedRandomValue() {
            return new Random().Next();
        }

        public static IPrincipal GetInjectedIPrincipalValue(ISession session) {
            return session.Principal;
        }

        // ReSharper disable once UnusedMember.Global
        // maybe called reflectively
        public static IQueryable<T> GetInjectedQueryableValue<T>(IObjectPersistor persistor) where T : class {
            return persistor.Instances<T>(false);
        }

        private static object GetParameterValue(this ParameterInfo p, INakedObjectAdapter adapter, ISession session, IObjectPersistor persistor) {
            if (p.Position == 0) {
                return adapter.Object;
            }

            if (p.GetCustomAttribute<InjectedAttribute>() != null) {
                var parameterType = p.ParameterType;
                if (parameterType == typeof(DateTime)) {
                    return GetInjectedDateTimeValue();
                }

                if (parameterType == typeof(Guid)) {
                    return GetInjectedGuidValue();
                }

                if (parameterType == typeof(int)) {
                    return GetInjectedRandomValue();
                }

                if (parameterType == typeof(IPrincipal)) {
                    return GetInjectedIPrincipalValue(session);
                }

                if (CollectionUtils.IsQueryable(parameterType)) {
                    var elementType = parameterType.GetGenericArguments().First();
                    var f = typeof(InjectUtils).GetMethod("GetInjectedQueryableValue")?.MakeGenericMethod(elementType);
                    return f?.Invoke(null, new object[] {persistor});
                }
            }

            return null;
        }

        public static object[] GetParameterValues(this MethodInfo method, INakedObjectAdapter adapter, ISession session, IObjectPersistor persistor) {
            return method.GetParameters().Select(p => p.GetParameterValue(adapter, session, persistor)).ToArray();
        }
    }
}