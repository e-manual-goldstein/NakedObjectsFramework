﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedFramework;

namespace NakedFunctions; 

/// <summary>
///     Applied to a string property or action parameter may contain carriage returns, and an optional default number of
///     lines and width,
///     which may be used by the display.
///     Applied to a function, indicates that the user may invoke the method multiple times in the form of a tabular input
///     form.
/// </summary>
public class MultiLineAttribute : AbstractMultiLineAttribute {
    public MultiLineAttribute() { }

    public MultiLineAttribute(int numberOfLines) : base(numberOfLines) { }

    public MultiLineAttribute(int numberOfLines, int width) : base(numberOfLines, width) { }
}