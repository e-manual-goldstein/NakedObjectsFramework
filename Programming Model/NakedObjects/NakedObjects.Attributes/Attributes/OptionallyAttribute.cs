// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects; 

/// <summary>
///     Used to indicate that either a property, or an action parameter, is optional.
/// </summary>
/// <remarks>
///     <para>
///         By default, the system assumes that all properties of an object are required, and therefore will
///         not let the user save a new object unless a value has been specified for each property. Similarly,
///         by default, the system assumes that all parameters in an action are required and will not let the
///         user execute that action unless values have been specified for each parameter.
///     </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class OptionallyAttribute : Attribute { }