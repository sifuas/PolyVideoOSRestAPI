using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

using Newtonsoft.Json;

namespace MEI.Integration.PolyVideoOSRestAPI.Objects
{
    public class PolyAPIAuthenticationData
    {
        [JsonProperty("user")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public PolyAPIAuthenticationData()
        {
            Username = "";
            Password = "";
        }
    }
}