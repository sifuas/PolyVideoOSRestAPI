
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// crestron libraries
using Crestron.SimplSharp;

// 3rd party libraries
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MEI.Integration.PolyVideoOSRestAPI.API_Objects
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