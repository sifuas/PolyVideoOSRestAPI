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

using MEI.Integration.PolyVideoOSRestAPI;
using MEI.Integration.PolyVideoOSRestAPI.InputCommands;



namespace MEI.Integration.PolyVideoOSRestAPI.Queue
{
    /// <summary>
    /// Class that handles the processing of APIInputCommands to be sent to the VideoOS API
    /// </summary>
    internal sealed class CommandProcessingQueue : ProcessingQueueThreadBase<APICommand>, ICCLDebuggable
    {
        // delegate to handle any errors in processing
        public delegate void ProcessingQueueErrorDelegate(object sender, Exception exception);
        public delegate void OnResultReceivedDelegate(object sender, CCLWebResponse response);

        // response to any errors that occur while processing the queue
        public event ProcessingQueueErrorDelegate OnQueueProcessingError;
        public event OnResultReceivedDelegate OnResponseReceived;

        // store the http connect to send the command with
        private APISession Session;

        /// <summary>
        /// Create a queue to processo API Commands using the given https connection.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="restClient"></param>
        public CommandProcessingQueue(string name, APISession session )
            : base(name)
        {
            Session = session;            
        }

        /// <summary>
        /// Process the command from the queue and send it to the API.
        /// A valid response will be added to the response processing queue
        /// </summary>
        /// <param name="command"></param>
        public override void ProcessQueueObject(APICommand command)
        {
            if (command == null || IsDisposed == true)
            {
                //CrestronConsole.PrintLine("{0}.ProcessQueueObject() [ {1} ] :  Exiting ProcessQueueObject( ). IsDisposed = {4}", this.GetType().Name, Name, IsDisposed);
                CCLDebug.PrintToConsole(eDebugLevel.Notice, "{0}.ProcessQueueObject() [ {1} ] :  Exiting ProcessQueueObject( ). IsDisposed = {4}", this.GetType().Name, Name, IsDisposed);
                return;
            }

            try
            {
                CCLDebug.PrintToConsole(eDebugLevel.Trace, "{0}.ProcessQueueObject() - Sending Command", this.GetType().Name);
                CCLDebug.PrintToConsole(eDebugLevel.Trace, command.ToString());

                // create the request
                CCLWebRequest request = new CCLWebRequest(Session.HostnameOrIPAddress, command.APIPath, 0, command.CommandRequestType, RequestAuthType.BASIC, null, null, command.CombineHeaders(Session.HTTPHeaders), command.GetFormattedCommandString(), command.TimeoutEnabled, command.Timeout, true, false, false, Encoding.UTF8);

                // send the request and get the response
                CCLWebResponse response = Session.Connection.SubmitRequest(request);

                CCLDebug.PrintToConsole(eDebugLevel.Trace, "{0}.ProcessQueueObject() [ {1} ] : Received Response - {2}", this.GetType().Name, Name, response);

                // add the response to the queue to be processed
                if (command.ProcessResponse)
                {                    
                    if( OnResponseReceived != null && response != null )
                        OnResponseReceived(this, response);
                    else if( response == null )
                        CCLDebug.PrintToConsole(eDebugLevel.Trace, "{0}.ProcessQueueObject() [ {1} ] :  NULL Response returned from REST client for command {2}", this.GetType().Name, Name, command.Path);
                }
            }
            catch (Crestron.SimplSharp.Net.Https.HttpsException httpsEx)
            {
                CCLDebug.PrintToConsole(eDebugLevel.Notice,"{0}.ProcessQueueObject() [ {1} ] :  Network Error Processing Queue", this.GetType().Name, Name);

                if( OnQueueProcessingError != null )
                    CrestronInvoke.BeginInvoke(thread => OnQueueProcessingError(this,httpsEx));
            }
            catch (Exception e)
            {                
                CrestronConsole.PrintLine("{0}.ProcessQueueObject() [ {1} ] :  Error Processing Queue - {2}", this.GetType().Name, Name, e.Message);
                CrestronConsole.PrintLine("{0}.ProcessQueueObject() [ {1} ] :  Error Processing Queue - {2}", this.GetType().Name, Name, e.StackTrace);
                ErrorLog.Error("{0}.ProcessQueueObject() [ {1} ] :  Error Processing Queue - {2}", this.GetType().Name, Name, e.Message);
                ErrorLog.Error("{0}.ProcessQueueObject() [ {1} ] :  Error Processing Queue - {2}", this.GetType().Name, Name, e.StackTrace);
            }
        }

        protected override void Dispose( bool disposing)
        {
            try
            {
                base.Dispose(disposing);
            }
            finally
            {
                Session = null;
                OnResponseReceived = null;
            }
        }
    }
}