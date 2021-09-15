using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// crestron libraries
using Crestron.SimplSharp;

using MEI.Integration.PolyVideoOSRestAPI.Logging;
using MEI.Integration.PolyVideoOSRestAPI.Network;
using MEI.Integration.PolyVideoOSRestAPI.Network.REST;

using MEI.Integration.PolyVideoOSRestAPI.Queue;

using MEI.Integration.PolyVideoOSRestAPI.ResponseHandlers;

namespace MEI.Integration.PolyVideoOSRestAPI.Queue
{
    internal sealed class ResponseHandlerProcessingQueue : ProcessingQueueThreadBase<CCLWebResponse>
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
        public override void ProcessQueueObject(CCLWebResponse response)
        {
            if (response == null || IsDisposed == true )
            {
                CCLDebug.PrintToConsole(eDebugLevel.Trace, "{0}.ProcessQueueObject() [ {1} ] :  Exiting ProcessQueueObject( ). IsDisposed = {4}", this.GetType().Name, Name, IsDisposed);
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
                CCLDebug.PrintToConsoleAndLog(eDebugLevel.Trace, "{0}.ProcessQueueObject() [ {1} ] :  Unhandled Response Received. Responsse = {2}", this.GetType().Name, Name, response);
                CCLDebug.PrintToConsoleAndLog(eDebugLevel.Trace, "{0}.ProcessQueueObject() [ {1} ] :  URL Path = {2}, Generic Path = {3}", this.GetType().Name, Name, urlPath, genericPath);

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