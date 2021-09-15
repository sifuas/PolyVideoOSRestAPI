using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// crestron libraries
using Crestron.SimplSharp;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PolyVideoOSRestAPI.Network;
using PolyVideoOSRestAPI.Logging;
using PolyVideoOSRestAPI;

namespace PolyVideoOSRestAPI.ResponseHandlers
{
    /// <summary>
    /// Handle responses from the API
    /// </summary>
    internal abstract class APIResponseHandlerBase : IDisposable
    {

        public EventHandler<APIUnknownResponseEventArgs> OnUnknownResponseReceived;
        public EventHandler<APIErrorResponseEventArgs> OnErrorResponseReceived;

        // the api path that this response handler is for, IE /rest/system/mode
        public string Path { get; private set; }

        // the api session associated with this handler
        public APISession Session { get; private set; }

        public APIResponseHandlerBase( APISession session ) 
        {
            Session = session;
        }
        
        public APIResponseHandlerBase( APISession session, string path) :this ( session )
        { 
            Path = path; 
        }

        /// <summary>
        /// Handle the response for the particular API Path
        /// </summary>
        /// <param name="result"></param>
        public virtual void HandleAPIResponse(WebResponse result)
        {
            if( result != null )
                Debug.PrintToConsole(eDebugLevel.Trace, "{0}.nVideoOSAPIResponseHandler() : Path = {1}, Result = {2}", this.GetType().Name, Path, result);
        }

        /// <summary>
        /// Call the handler for an unknown response for the given handler
        /// </summary>
        /// <param name="response"></param>
        public void HandleUnknownAPIResponse(WebResponse response)
        {
            if (OnUnknownResponseReceived != null)
                OnUnknownResponseReceived(this, new APIUnknownResponseEventArgs(response));
        }

        /// <summary>
        /// Call the handler for any server errors for the given handler
        /// </summary>
        /// <param name="response"></param>
        public void HandleErrorAPIResponse(WebResponse response)
        {
            if (OnErrorResponseReceived != null)
                OnErrorResponseReceived(this, new APIErrorResponseEventArgs(response));
        }

        /// <summary>
        /// Get the full API path based on the REST base path
        /// </summary>
        /// <returns></returns>
        public string GetFullAPIPath()
        {
            return APIGlobal.CreateFullRESTAPIPath(Path);
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            Session = null;
            OnUnknownResponseReceived = null;
            OnErrorResponseReceived = null;
        }

        #endregion
    }

    
}