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

// crestron librarires
using Crestron.SimplSharp;

// 3rd party libraries
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PolyVideoOSRestAPI.Logging;
using PolyVideoOSRestAPI.Network;
using PolyVideoOSRestAPI.Network.REST;
using PolyVideoOSRestAPI.InputCommands;

namespace PolyVideoOSRestAPI
{
    internal sealed class APISession : APIInputCommand, IDisposable, IDebuggable
    {
        // the client to connect to
        public GenericRestClientBase Connection { get; internal set; }

        // store headers needed to communicate with the API
        public Dictionary<string, string> HTTPHeaders { get; internal set; }

        // is the api session connected?
        public bool Connected { get; internal set; }

        // indicates whether the system should be connected even if it isn't
        public bool ConnectionRequested { get; internal set; }

        // the host to connect to for the session
        public String HostnameOrIPAddress { get; set; }

        // track the state of the session
        public eSessionState State { get; internal set; }

        // the ID for the current session
        private string _sessionId = "";
        public string SessionID 
        {
            get
            {
                if (_sessionId == null)
                    return "";
                else
                    return _sessionId;
            }
            internal set
            {
                _sessionId = value;
            }
        }

        // store the authentication data used for this session
        public APIAuthenticationCredentials Credentials { get; private set; }

        public APISession()
        {            
            Credentials          = new APIAuthenticationCredentials();
            Connection           = new HttpsRESTClient();
            HTTPHeaders          = new Dictionary<string, string>();
        }

        /// <summary>
        /// Test if the authentication data for the session is valid
        /// </summary>
        /// <returns></returns>
        public bool AuthenticationIsValid()
        {
            return (!String.IsNullOrEmpty(Credentials.Username) && !String.IsNullOrEmpty(Credentials.Password));
        }

        /// <summary>
        /// Return the session ID as a formatted HTTP cookie
        /// </summary>
        /// <returns></returns>
        public string GetSessionCookie()
        {
            return String.Format("session_id={0}", SessionID);
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder( String.Format( "Session : onnected = {0}, ConnectedRequested = {1}, Username = {2}, Password = {3}, SessionID = ", Connected, ConnectionRequested, Credentials.Username, Credentials.Password ));
            if( SessionID != null )
                str.Append(SessionID);
            else
                str.Append("NULL");

            return str.ToString();
        }

        #region VideoOSInputCommandObject Members

        /// <summary>
        /// Convert this object into the appropriate string for the input type
        /// </summary>
        /// <param name="inputType">The type</param>
        /// <returns></returns>
        public string ToInputCommandString( eCommandInputFormatType inputType )
        {
            string inputCommandString = "";

            switch( inputType )
            {
                case eCommandInputFormatType.BODY_JSON:
                    inputCommandString = new JObject(
                                    new JProperty("user", Credentials.Username),
                                    new JProperty("password", Credentials.Password)
                                    ).ToString();
                    break;
            }

            return inputCommandString;
        }

        #endregion





        #region IDisposable Members

        public void Dispose()
        {
            try
            {                
                if (HTTPHeaders != null)
                    HTTPHeaders.Clear();
            }
            finally
            {
                Credentials = null;
                Connection = null;
                HTTPHeaders = null;
            }
        }

        #endregion

        #region IDebuggable Members

        public void PrintDebugState()
        {
            CrestronConsole.PrintLine(this.ToString());
        }

        #endregion
    }
}