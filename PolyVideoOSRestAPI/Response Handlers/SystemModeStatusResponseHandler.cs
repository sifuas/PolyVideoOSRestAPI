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
    internal class SystemModeStatusResponseHandler : APIResponseHandlerBase
    {
        public EventHandler<APISystemModeStateEventArgs> OnSystemModeChanged;

        internal SystemModeStatusResponseHandler( APISession session, string path)
            : base(session, path)
        {
        }

        public override void HandleAPIResponse(WebResponse response)
        {
            if (response == null)
            {                
                ProjectDebug.PrintToConsole(eDebugLevel.Trace, "{0}.HandleAPIResponse() : NULL web response", this.GetType().Name);
                return;
            }            

            try
            {
                // if OK response then 
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ProjectDebug.PrintToConsole(eDebugLevel.Trace, "{0}.HandleAPIResponse() : Received OK Response", this.GetType().Name);
                    APISystemModeObject systemMode = JsonConvert.DeserializeObject<APISystemModeObject>(response.Content);

                    // send the event
                    EventHandler<APISystemModeStateEventArgs> handler = OnSystemModeChanged;
                    if (handler != null)
                        handler(this, new APISystemModeStateEventArgs(systemMode, response));
                }
                else if ((response.StatusCode >= HttpStatusCode.BadRequest) && (response.StatusCode < HttpStatusCode.InternalServerError)) // client error that we are not handling is returned
                {
                    ProjectDebug.PrintToConsoleAndLog(eDebugLevel.Warning, "{0}.HandleAPIResponse() : Received Unknown Response - Status = {1} Content = {2}", this.GetType().Name, response.StatusCode, response.Content);
                    base.HandleUnknownAPIResponse(response);
                }
                else if ((response.StatusCode >= HttpStatusCode.InternalServerError) && (response.StatusCode <= HttpStatusCode.HttpVersionNotSupported))
                {
                    ProjectDebug.PrintToConsoleAndLog(eDebugLevel.Warning, "{0}.HandleAPIResponse() : Received Error Response - Status = {1} Content = {2}", this.GetType().Name, response.StatusCode, response.Content);
                    base.HandleErrorAPIResponse(response);
                }
            }
            catch (Exception e)
            {
                ProjectDebug.PrintExceptionToConsoelAndLog(eDebugLevel.Error, e, "{0}.HandleAPIResponse( ) - Error Processing Response");
                ProjectDebug.PrintToConsoleAndLog(eDebugLevel.Error, response.ToString());
            }
        }

        public override void Dispose()
        {
            OnSystemModeChanged = null;
            base.Dispose();
        }
    }
}