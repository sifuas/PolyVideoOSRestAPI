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

using PolyVideoOSRestAPI.Logging;
using PolyVideoOSRestAPI.Network;
using PolyVideoOSRestAPI.Network.REST;
using PolyVideoOSRestAPI.Queue;
using PolyVideoOSRestAPI.ResponseHandlers;

namespace PolyVideoOSRestAPI.Queue
{
    internal sealed class ResponseHandlerProcessingQueue : ProcessingQueueThreadBase<WebResponse>
    {
       
        // store a reference to the collection of response handlers
        private Dictionary<string, APIResponseHandlerBase> responseHandlersCollection;

        /// <summary>
        /// Create a queue to processo API Commands using the given https connection.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="restClient"></param>
        public ResponseHandlerProcessingQueue( string name, Dictionary<string, APIResponseHandlerBase> responeHandlers )
            : base(name)
        {
            responseHandlersCollection = responeHandlers;
        }

        /// <summary>
        /// Process the object from the queue using the provided response handler.
        /// If a specific response handler is found it will be called on a new thread.
        /// If a specific response handler is not found then
        /// </summary>
        /// <param name="command"></param>
        public override void ProcessQueueObject(WebResponse response)
        {
            if (response == null || IsDisposed == true )
            {
                Debug.PrintToConsole(eDebugLevel.Trace, "{0}.ProcessQueueObject() [ {1} ] :  Exiting ProcessQueueObject( ). IsDisposed = {4}", this.GetType().Name, Name, IsDisposed);
                return;
            }

            string urlPath = NetworkHelperFunctions.GetURLPath(response.ResponseURL);
            string genericPath = APIGlobal.CreateFullRESTAPIPath("*");             

            APIResponseHandlerBase responseHandler = null;

            // look for a specific handler for the response
            if ( responseHandlersCollection.ContainsKey(urlPath) )
            {
                responseHandler = responseHandlersCollection[urlPath];               
            }
            else if( responseHandlersCollection.ContainsKey( genericPath ) )
            {
                responseHandler = responseHandlersCollection[genericPath];  
            }

            // if a response handler was found pass on the message in a new thread
            if (responseHandler != null)
            {
                CrestronInvoke.BeginInvoke(thread => responseHandler.HandleAPIResponse(response));
            }
            else
            {                
                Debug.PrintToConsoleAndLog(eDebugLevel.Trace, "{0}.ProcessQueueObject() [ {1} ] :  Unhandled Response Received. Responsse = {2}", this.GetType().Name, Name, response);
                Debug.PrintToConsoleAndLog(eDebugLevel.Trace, "{0}.ProcessQueueObject() [ {1} ] :  URL Path = {2}, Generic Path = {3}", this.GetType().Name, Name, urlPath, genericPath);

                foreach (string key in responseHandlersCollection.Keys)
                {
                    CrestronConsole.PrintLine(" Key = {0}", key);
                }                
            }
        }

        /// <summary>
        /// Dispose of any class members as needed then dispose of the base
        /// </summary>
        protected override  void Dispose( bool disposing )
        {
            try
            {
                base.Dispose(disposing);
            }
            finally
            {
                responseHandlersCollection = null;
            }
        }
    }
}