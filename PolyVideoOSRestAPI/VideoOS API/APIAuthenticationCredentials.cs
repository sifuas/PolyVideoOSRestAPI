using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

using Newtonsoft.Json;

namespace PolyVideoOSRestAPI
{
    internal class APIAuthenticationCredentials
    {
        [JsonProperty("user")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        public APIAuthenticationCredentials()
        {
            Username = "";
            Password = "";
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append("Username = ");
            if( Username != null )                          
                str.Append(Username);
            
            str.Append(", Password = ");
            if (Password != null)
                str.Append(Password);

            return str.ToString();
        }
    }
}