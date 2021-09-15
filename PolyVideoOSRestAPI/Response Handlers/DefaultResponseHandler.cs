using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Crestron.SimplSharp;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PolyVideoOSRestAPI.Network;
using PolyVideoOSRestAPI.Logging;
using PolyVideoOSRestAPI;

namespace PolyVideoOSRestAPI.ResponseHandlers
{
    internal class DefaultResponseHandler : APIResponseHandlerBase
    {
        public EventHandler<APIGenericStateEventArgs> OnDefaultResponseHandlerResponseReceived;

        public DefaultResponseHandler( APISession session, string path)
            : base(session, path)
        {
        }

        public override void HandleAPIResponse(WebResponse result)
        {
            if (result == null)
            {
                Debug.PrintToConsole(eDebugLevel.Trace, "{0}.HandleAPIResponse() : NULL web response", this.GetType().Name );
                return;
            }           

            EventHandler<APIGenericStateEventArgs> handler = OnDefaultResponseHandlerResponseReceived;
            if (handler != null)
                handler(this, new APIGenericStateEventArgs(result));

        }

        public override void Dispose()
        {
            OnDefaultResponseHandlerResponseReceived = null;
            base.Dispose();            
        }

    }
}