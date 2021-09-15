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
    internal class SystemModeStatusResponseHandler : APIResponseHandlerBase
    {
        public EventHandler<APISystemModeStateEventArgs> OnSystemModeChanged;

        internal SystemModeStatusResponseHandler( APISession session, string path)
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

            try
            {
                // if OK response then 
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    CCLDebug.PrintToConsole(eDebugLevel.Trace, "{0}.HandleAPIResponse() : Received OK Response", this.GetType().Name);
                    APISystemModeObject systemMode = JsonConvert.DeserializeObject<APISystemModeObject>(response.Content);

                    // send the event
                    EventHandler<APISystemModeStateEventArgs> handler = OnSystemModeChanged;
                    if (handler != null)
                        handler(this, new APISystemModeStateEventArgs(systemMode, response));
                }
                else if ((response.StatusCode >= HttpStatusCode.BadRequest) && (response.StatusCode < HttpStatusCode.InternalServerError)) // client error that we are not handling is returned
                {
                    CCLDebug.PrintToConsoleAndLog(eDebugLevel.Warning, "{0}.HandleAPIResponse() : Received Unknown Response - Status = {1} Content = {2}", this.GetType().Name, response.StatusCode, response.Content);
                    base.HandleUnknownAPIResponse(response);
                }
                else if ((response.StatusCode >= HttpStatusCode.InternalServerError) && (response.StatusCode <= HttpStatusCode.HttpVersionNotSupported))
                {
                    CCLDebug.PrintToConsoleAndLog(eDebugLevel.Warning, "{0}.HandleAPIResponse() : Received Error Response - Status = {1} Content = {2}", this.GetType().Name, response.StatusCode, response.Content);
                    base.HandleErrorAPIResponse(response);
                }
            }
            catch (Exception e)
            {
                CCLDebug.PrintExceptionToConsoelAndLog(eDebugLevel.Error, e, "{0}.HandleAPIResponse( ) - Error Processing Response");
                CCLDebug.PrintToConsoleAndLog(eDebugLevel.Error, response.ToString());
            }
        }

        public override void Dispose()
        {
            OnSystemModeChanged = null;
            base.Dispose();
        }
    }
}