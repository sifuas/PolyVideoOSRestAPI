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