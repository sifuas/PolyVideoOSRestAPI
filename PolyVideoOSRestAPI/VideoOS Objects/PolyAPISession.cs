using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using MEI.Integration.PolyVideoOSRestAPI.InputCommands;

namespace MEI.Integration.PolyVideoOSRestAPI.Objects
{
    public class VideoOSAPISession : VideoOSInputCommandObject
    {
        // is the api session connected?
        public bool Connected { get; set; }

        // has the connection been made before?
        public bool HasConnectedBefore { get; set; }
        
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
            set
            {
                _sessionId = value;
            }
        }

        public PolyAPIAuthenticationData AuthenticationData { get; private set; }

        public VideoOSAPISession()
        {            
            AuthenticationData = new PolyAPIAuthenticationData();
        }

        public bool AuthenticationIsValid()
        {
            return (!String.IsNullOrEmpty(AuthenticationData.Username) && !String.IsNullOrEmpty(AuthenticationData.Password));
        }

        public string GetSessionCookie()
        {
            return String.Format("session_id={0}", SessionID);
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder( String.Format( "Connected = {0}, HasConnected = {1}, Username = {2}, Password = {3}, SessionID = ", Connected, HasConnectedBefore, AuthenticationData.Username, AuthenticationData.Password ));
            if( SessionID != null )
                str.Append(SessionID);
            else
                str.Append("NULL");

            return str.ToString();
        }

        #region VideoOSInputCommandObject Members

        /// <summary>
        /// Convert this object into the appropriate strng for the input type
        /// </summary>
        /// <param name="inputType">The type</param>
        /// <returns></returns>
        public string ToInputCommandString( VideoOSAPICommandInputFormatType inputType )
        {
            string inputCommandString = "";

            switch( inputType )
            {
                case VideoOSAPICommandInputFormatType.BODY_JSON:
                    inputCommandString = new JObject(
                                    new JProperty("user", AuthenticationData.Username),
                                    new JProperty("password", AuthenticationData.Password)
                                    ).ToString();
                    break;
            }

            return inputCommandString;
        }

        #endregion
    }
}