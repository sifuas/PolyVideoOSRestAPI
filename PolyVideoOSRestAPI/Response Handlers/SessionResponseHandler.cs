using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Crestron.SimplSharp;
using RequestType = Crestron.SimplSharp.Net.Https.RequestType;

using MEI.Integration.PolyVideoOSRestAPI.Network;
using MEI.Integration.PolyVideoOSRestAPI.Logging;
using HttpStatusCode = MEI.Integration.PolyVideoOSRestAPI.Network.CCLHttpStatusCode;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using MEI.Integration.PolyVideoOSRestAPI;
using MEI.Integration.PolyVideoOSRestAPI.API_Objects;

namespace MEI.Integration.PolyVideoOSRestAPI.ResponseHandlers
{
    /// <summary>
    /// Handle responses for POST requests to /session
    /// </summary>
    internal class SessionResponseHandler : APIResponseHandlerBase
    {
        public EventHandler<APISessionStateEventArgs> OnSessionStateChanged;

        public SessionResponseHandler(APISession session, string path)
            : base(session, path)
        {
        }

        public override void HandleAPIResponse(CCLWebResponse response)
        {
            if (response == null)
            {                
                CCLDebug.PrintToConsole(eDebugLevel.Trace, "{0}.HandleAPIResponse() : NULL web response", this.GetType().Name);
                return;
            }
           
            eSessionState state = eSessionState.LOGIN_ERROR;

            // store response data
            HttpStatusCode code = response.StatusCode;
            string content          = response.Content;
            RequestType requestType = response.OriginalRequestType;
            string sessionID        = "";

            try
            {

                // convert the data into an object we can use
                APISessionStateObject sessionState = JsonConvert.DeserializeObject<APISessionStateObject>(content);

                // logged in
                if ((code == HttpStatusCode.OK) && sessionState.Success && (requestType == RequestType.Post))
                {
                    if (String.IsNullOrEmpty(sessionState.Session.SessionID))
                    {
                        CCLDebug.PrintToConsole(eDebugLevel.Trace, "{0}.HandleAPIResponse() : No Session ID Returned in response {1}", this.GetType().Name, content);
                        return;
                    }
                    else
                    {
                        state = eSessionState.LOGGED_IN;
                        sessionID = sessionState.Session.SessionID;
                    }
                }
                // log out successful
                else if ((code == HttpStatusCode.OK) && (requestType == RequestType.Delete))
                {
                    state = eSessionState.LOGGED_OUT;
                }
                else if (code == HttpStatusCode.BadRequest) // bad request, need to check for current login state
                {

                    if (sessionState.Reason == null)
                    {
                        CCLDebug.PrintToConsole(eDebugLevel.Warning, "{0}.HandleAPIResponse() :  HTTP Response 400. No 'reason' field returned in VideoOS API JSON response", this.GetType().Name);
                        return;
                    }

                    // already logged in
                    if (sessionState.Reason.Equals("SessionAlreadyActive"))
                    {
                        state = eSessionState.LOGGED_IN;
                    }
                    // invalid credentials
                    else if (sessionState.Reason.Equals("SessionRequestInvalid"))
                    {
                        CCLDebug.PrintToConsoleAndLog(eDebugLevel.Warning, "{0}.HandleAPIResponse() : Invalid Login Credentials", this.GetType().Name);
                        state = eSessionState.INVALID_CREDENTIALS;
                    }
                    // malformed request
                    else if (sessionState.Reason.Equals("SessionRequestInvalid"))
                    {
                        CCLDebug.PrintToConsoleAndLog(eDebugLevel.Warning, "{0}.HandleAPIResponse() : Invalid Login Credentials", this.GetType().Name);
                        state = eSessionState.INVALID_CREDENTIALS;
                    }
                    // locked out from too many failed login attempts
                    else if (sessionState.Reason.Equals("SessionPortLockout"))
                    {
                        CCLDebug.PrintToConsoleAndLog(eDebugLevel.Warning, "{0}.HandleAPIResponse() : Too many attempts, session locked out", this.GetType().Name);
                        state = eSessionState.LOCKED_OUT;
                    }
                    // no valid session to connect to, logout to trigger re-login if needed
                    else if (sessionState.Reason.Equals("SessionNoActiveSession"))
                    {
                        CCLDebug.PrintToConsoleAndLog(eDebugLevel.Warning, "{0}.HandleAPIResponse() : Already Logged Out", this.GetType().Name);
                        state = eSessionState.LOGGED_OUT;
                    }
                    // unknown
                    else
                    {
                        CCLDebug.PrintToConsoleAndLog(eDebugLevel.Warning, "{0}.HandleAPIResponse() : Unhandled Reason {1}", this.GetType().Name, sessionState.Reason);
                        state = eSessionState.LOGIN_ERROR;
                    }

                }
                else  // login failure
                {
                    state = eSessionState.LOGIN_ERROR;
                }

                // send the event
                EventHandler<APISessionStateEventArgs> handler = OnSessionStateChanged;
                if (handler != null)
                    handler(this, new APISessionStateEventArgs(state, sessionID, response, sessionState));
            }
            catch (Exception e)
            {                
                CCLDebug.PrintExceptionToConsoelAndLog(eDebugLevel.Error,e, "{0}.HandleAPIResponse( ) - Error Processing Response");               
                CCLDebug.PrintToConsoleAndLog(eDebugLevel.Error, response.ToString());
            }
        }

        public override void Dispose()
        {
            OnSessionStateChanged = null;
            base.Dispose();
        }
    }
}