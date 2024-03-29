﻿/*
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
    public class APISessionStateObject : APIObjectBase
    {

        [JsonProperty("loginStatus")]
        public InternalSessionStatus LoginStatus { get; set; }

        [JsonProperty("session")]
        public InternalSessionState Session { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonIgnore]
        public ushort UShortSuccess
        {
            get { return (ushort)(Success ? 1 : 0); }
        }

        // default constructor for simpl+
        public APISessionStateObject() { }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("Session : ");
            str.Append("\nSuccess - " + Success);
            str.Append("\nReason - " + Reason.EmptyIfNull());
            if (LoginStatus != null)
                str.Append(LoginStatus.ToString());

            if (Session != null)
                str.Append(Session.ToString());

            return str.ToString();
        }

        /// <summary>
        /// Define internal classes to hold data
        /// </summary>
        public class InternalSessionStatus
        {
            [JsonProperty("failedLogins")]
            public ushort FailedLogins {get;set;}

            [JsonProperty("isPasswordAgeLimitReached")]
            public bool PasswordAgeLimitReached {get;set;}

            [JsonProperty("lastLoginClient")]
            public string LastLoginClient {get;set;}

            [JsonProperty("lastLoginClientType")]
            public string LastLoginClientType {get;set;}

            [JsonProperty("lastLoginTime")]
            public long LastLoginTime {get;set;}

            [JsonProperty("loginResult")]
            public string LoginResult {get;set;}

            public override string ToString()
            {
                StringBuilder str = new StringBuilder();
                str.Append("\n\nSession Status :");
                str.Append("\nFailed Logins - " + FailedLogins);
                str.Append("\nAge Limit - " + PasswordAgeLimitReached);
                str.Append("\nLastLoginClient - " + LastLoginClient.EmptyIfNull());
                str.Append("\nLastLoginTime - " + LastLoginTime);
                str.Append("\nLoginResult - " + LoginResult.EmptyIfNull());
                return str.ToString();
            }
        }

        public class InternalSessionState
        {
            [JsonProperty("clientType")]
            public string ClientType {get;set;}

            [JsonProperty("creationTime")]
            public long CreationTime {get;set;}

            [JsonProperty("isAuthenticated")]
            public bool IsAuthenticated { get;set;}

            [JsonProperty("isConnected")]
            public bool IsConnected {get;set;}

            [JsonProperty("isNew")]
            public bool IsNew {get;set;}

            [JsonProperty("isStale")]
            public bool IsStale {get;set;}

            [JsonProperty("location")]
            public string Location {get;set;}

            [JsonProperty("role")]
            public string Role {get;set;}

            [JsonProperty("sessionId")]
            public string SessionID {get;set;}

            [JsonProperty("userId")]
            public string UserID { get; set; }

            public override string ToString()
            {
                StringBuilder str = new StringBuilder();
                str.Append("\n\nSession State :");
                str.Append("\nClient TYpe - " + ClientType.EmptyIfNull());
                str.Append("\nCreation Time - " + CreationTime);
                str.Append("\nIsAuthenticated - " + IsAuthenticated);
                str.Append("\nIsConnected - " + IsConnected);
                str.Append("\nIsNew - " + IsNew);
                str.Append("\nIsStale - " + IsStale);
                str.Append("\nLocation - " + Location.EmptyIfNull());
                str.Append("\nRole - " + Role.EmptyIfNull());
                str.Append("\nSessionID - " + SessionID.EmptyIfNull());
                str.Append("\nUserID - " + UserID.EmptyIfNull());

                return str.ToString();
            }

        }
    }
}