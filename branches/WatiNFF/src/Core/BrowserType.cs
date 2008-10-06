#region WatiN Copyright (C) 2006-2008 Jeroen van Menen

//Copyright 2006-2008 Jeroen van Menen
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Text;

namespace WatiN.Core
{
    /// <summary>
    /// Represents the types of browsers that can be used to run tests.
    /// </summary>
    public enum BrowserType
    {
        /// <summary>
        /// The installed version of Internet explorer
        /// </summary>
        InternetExplorer,

        /// <summary>
        /// The installed version of FireFox
        /// </summary>
        FireFox
    }
}