using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// crestron libraries
using Crestron.SimplSharp;

// 3rd party libraries
using Newtonsoft.Json;

using MEI.Integration.PolyVideoOSRestAPI.Network;

using MEI.Integration.PolyVideoOSRestAPI.API_Objects;

namespace MEI.Integration.PolyVideoOSRestAPI
{

    /// <summary>
    /// Events to indicate the state from a response to an API call
    /// </summary>
    public abstract class APIEventArgsBase : EventArgs
    {
        public CCLWebResponse Response { get; protected set; }
    }

    /// <summary>
    /// When a response handler receives a response that it can't handle
    /// </summary>
    public class APIUnknownResponseEventArgs : APIEventArgsBase
    {
        public APIUnknownResponseEventArgs(CCLWebResponse response)
        {
            Response = response;
        }
    }

    /// <summary>
    /// When a response handler receives a server error response
    /// </summary>
    public class APIErrorResponseEventArgs : APIEventArgsBase
    {
        public APIErrorResponseEventArgs(CCLWebResponse response)
        {
            Response = response;
        }
    }

    /// <summary>
    /// Event indicating that the state of the login session has changed.
    /// </summary>
    public class APISessionStateEventArgs : APIEventArgsBase
    {
        public eSessionState State { get; private set; }
        public string SessionID { get; private set; }
        public string StringState { get; private set; }

        public APISessionStateObject SessionState { get; private set; }

        public ushort UShortState
        {
            get
            {
                return ((ushort)State);
            }
        }

        // default constructor for simpl+
        public APISessionStateEventArgs() { }

        public APISessionStateEventArgs(eSessionState state, string sessionID, CCLWebResponse response, APISessionStateObject sessionState)
        {
            State = state;
            SessionID = sessionID;
            Response = response;
            SessionState = sessionState;
            StringState = State.ToString();
        }
    }

    /// <summary>
    /// Event indicating when the device mode status changes
    /// </summary>
    public class APISystemModeStateEventArgs : APIEventArgsBase
    {
        public APISystemModeObject SystemMode { get; private set; }

        // default constructor for simpl+
        public APISystemModeStateEventArgs() { }

        public APISystemModeStateEventArgs(APISystemModeObject systemMode, CCLWebResponse response)
        {
            SystemMode = systemMode;
            Response = response;
        }
    }

    /// <summary>
    /// Event to handle generic responses
    /// </summary>
    public class APIGenericStateEventArgs : APIEventArgsBase
    {
        // default constructor for simpl+
        public APIGenericStateEventArgs() { }

        public APIGenericStateEventArgs(CCLWebResponse response)
        {
            Response = response;
        }

        public ushort GetResponseCode()
        {
            if (Response != null)
                return (ushort)Response.StatusCode;
            else
                return 0;
        }

        public string GetResponseContent()
        {
            if (Response != null)
                return Response.Content;
            else
                return "";
        }

        public string GetResponseURL()
        {
            if (Response != null)
                return Response.ResponseURL;
            else
                return "";
        }

        public int GetHeaderCount()
        {
            if (Response != null && Response.Headers != null)
                return Response.Headers.Count;
            else
                return 0;
        }

        public string GetHeaderKey(int index)
        {
            string headerKey = "";

            if ((Response != null) && (Response.Headers != null) && (Response.Headers.Keys.Count > index ))
            {
                headerKey = Response.Headers.Keys.ElementAt(index);
            }

            return headerKey;
        }

        public string GetHeaderValue(int index)
        {
            string headerValue = "";            

            if ((Response != null) && (Response.Headers != null) && (Response.Headers.Keys.Count > index))
            {
                headerValue = Response.Headers[GetHeaderKey(index)];
            }

            return headerValue;
        }
    }
}