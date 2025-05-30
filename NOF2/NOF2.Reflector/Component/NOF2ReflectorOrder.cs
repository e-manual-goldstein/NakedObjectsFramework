﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Component;
using NakedFramework.ParallelReflector.Component;

namespace NOF2.Reflector.Component;

public class NOF2ReflectorOrder<T> : IReflectorOrder<T> {
    public int Order => typeof(T) switch {
        { } t when t.IsAssignableTo(typeof(SystemTypeReflector)) => 0,
        { } t when t.IsAssignableTo(typeof(NOF2Reflector)) => 1,
        _ => 2
    };
}