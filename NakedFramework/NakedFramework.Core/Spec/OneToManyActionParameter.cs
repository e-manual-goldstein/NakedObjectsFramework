// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;

namespace NakedFramework.Core.Spec;

public sealed class OneToManyActionParameter : ActionParameterSpec, IOneToManyActionParameterSpec {
    public OneToManyActionParameter(int index, IActionSpec actionImpl, IActionParameterSpecImmutable actionParameterSpecImmutable, INakedFramework framework)
        : base(index, actionImpl, actionParameterSpecImmutable, framework) { }
}