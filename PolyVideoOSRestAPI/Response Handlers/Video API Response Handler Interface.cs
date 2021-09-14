using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// crestron libraries
using Crestron.SimplSharp;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using CommonCoreLibrary.Network;
using CommonCoreLibrary.Logging;

using MEI.Integration.PolyVideoOSRestAPI.Events;

namespace MEI.Integration.PolyVideoOSRestAPI.ResponseHandlers
{
    /// <summary>
    /// Handle responses from the API
    /// </summary>
    public abstract class VideoOSAPIResponseHandlerBase
    {      
        public string Path { get; private set; }

        public VideoOSAPIResponseHandlerBase() { }
        public VideoOSAPIResponseHandlerBase(string path) { Path = path; }

        public virtual void HandleAPIResponse(WebResponseResult result)
        {
            if( result != null )
                Debug.PrintToConsole(eDebugLevel.Trace, "\nVideoOSAPIResponseHandler ( {0} ) : Result = {1}", Path, result);
        }
    }

    
}