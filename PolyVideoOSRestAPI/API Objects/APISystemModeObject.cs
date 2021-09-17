/*
// The MIT License (MIT)
// Copyright (c) 2021 Andrew Ross
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software
// and associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
*/ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// crestron libraries
using Crestron.SimplSharp;

// 3rd party libraries
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PolyVideoOSRestAPI.API_Objects
{

    /// <summary>
    /// Information about the current system mode
    /// </summary>
    public class APISystemModeObject : APIObjectBase
    {
        [JsonProperty("activePersona")]
        public string ActivePersona { get; set; }

        [JsonProperty("app")]
        public string App { get; set; }

        [JsonProperty("appLocked")]
        public bool AppLocked { get; set; }

        [JsonProperty("appPersona")]
        public string AppPersona { get; set; }

        [JsonProperty("ecoMode")]
        public string EcoMode { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("polyCallControl")]
        public bool PolyCallControl { get; set; }

        [JsonIgnore]
        public bool DeviceModeEnabled
        {
            get
            {
                if (ActivePersona != null)
                    return ActivePersona.ToLower().Equals("camuvc");
                else
                    return false;
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("activePersona = " + ActivePersona);
            str.Append(", app = " + App);
            str.Append(", appLocked = " + AppLocked);
            str.Append(", appPersona = " + AppPersona);
            str.Append(", ecoMode = " + EcoMode);
            str.Append(", mode = " + Mode);
            str.Append(", polyCallControl = " + PolyCallControl);

            return str.ToString();
        }
    }
}