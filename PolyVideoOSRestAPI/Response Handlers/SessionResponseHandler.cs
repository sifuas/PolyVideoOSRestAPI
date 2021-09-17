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

using Crestron.SimplSharp;
using RequestType = Crestron.SimplSharp.Net.Https.RequestType;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PolyVideoOSRestAPI.Network;
using PolyVideoOSRestAPI.Logging;
using HttpStatusCode = PolyVideoOSRestAPI.Network.HttpStatusCode;
using PolyVideoOSRestAPI;
using PolyVideoOSRestAPI.API_Objects;

namespace PolyVideoOSRestAPI.ResponseHandlers
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

        public override void HandleAPIResponse(WebResponse response)
        {
            if (response == null)
            {                
                Debug.PrintToConsole(eDebugLevel.Trace, "{0}.HandleAPIResponse() : NULL web response", this.GetType().Name);
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
                        Debug.PrintToConsole(eDebugLevel.Trace, "{0}.HandleAPIResponse() : No Session ID Returned in response {1}", this.GetType().Name, content);
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
                        Debug.PrintToConsole(eDebugLevel.Warning, "{0}.HandleAPIResponse() :  HTTP Response 400. No 'reason' field returned in VideoOS API JSON response", this.GetType().Name);
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
                        Debug.PrintToConsoleAndLog(eDebugLevel.Warning, "{0}.HandleAPIResponse() : Invalid Login Credentials", this.GetType().Name);
                        state = eSessionState.INVALID_CREDENTIALS;
                    }
                    // malformed request
                    else if (sessionState.Reason.Equals("SessionRequestInvalid"))
                    {
                        Debug.PrintToConsoleAndLog(eDebugLevel.Warning, "{0}.HandleAPIResponse() : Invalid Login Credentials", this.GetType().Name);
                        state = eSessionState.INVALID_CREDENTIALS;
                    }
                    // locked out from too many failed login attempts
                    else if (sessionState.Reason.Equals("SessionPortLockout"))
                    {
                        Debug.PrintToConsoleAndLog(eDebugLevel.Warning, "{0}.HandleAPIResponse() : Too many attempts, session locked out", this.GetType().Name);
                        state = eSessionState.LOCKED_OUT;
                    }
                    // no valid session to connect to, logout to trigger re-login if needed
                    else if (sessionState.Reason.Equals("SessionNoActiveSession"))
                    {
                        Debug.PrintToConsoleAndLog(eDebugLevel.Warning, "{0}.HandleAPIResponse() : Already Logged Out", this.GetType().Name);
                        state = eSessionState.LOGGED_OUT;
                    }
                    // unknown
                    else
                    {
                        Debug.PrintToConsoleAndLog(eDebugLevel.Warning, "{0}.HandleAPIResponse() : Unhandled Reason {1}", this.GetType().Name, sessionState.Reason);
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
                Debug.PrintExceptionToConsoelAndLog(eDebugLevel.Error,e, "{0}.HandleAPIResponse( ) - Error Processing Response");               
                Debug.PrintToConsoleAndLog(eDebugLevel.Error, response.ToString());
            }
        }

        public override void Dispose()
        {
            OnSessionStateChanged = null;
            base.Dispose();
        }
    }
}