﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedFramework.Architecture.Adapter;

/// <summary>
///     Indicates that the implementing class should be able to be encoded to an array of strings
/// </summary>
/// <remarks>
///     <para>
///         The implementing class should also have a constructor that accepts an array of strings and
///         initializes from them
///     </para>
/// </remarks>
public interface IEncodedToStrings {
    string[] ToEncodedStrings();
    string[] ToShortEncodedStrings();
}