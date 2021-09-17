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

using PolyVideoOSRestAPI;
using PolyVideoOSRestAPI.InputCommands;

namespace PolyVideoOSRestAPI.Queue
{
    /// <summary>
    /// Class that handles the processing of APIInputCommands to be sent to the VideoOS API
    /// </summary>
    internal sealed class CommandProcessingQueue : ProcessingQueueThreadBase<APICommand>, IDebuggable
    {
        // delegate to handle any errors in processing
        public delegate void ProcessingQueueErrorDelegate(object sender, Exception exception);
        public delegate void OnResultReceivedDelegate(object sender, WebResponse response);

        // response to any errors that occur while processing the queue
        public event ProcessingQueueErrorDelegate OnQueueProcessingError;
        public event OnResultReceivedDelegate OnResponseReceived;

        // store the session to send the command with
        private APISession Session;

        /// <summary>
        /// Create a queue to processo API Commands using the given session.
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
                Debug.PrintToConsole(eDebugLevel.Notice, "{0}.ProcessQueueObject() [ {1} ] :  Exiting ProcessQueueObject( ). IsDisposed = {4}", this.GetType().Name, Name, IsDisposed);
                return;
            }

            try
            {
                Debug.PrintToConsole(eDebugLevel.Trace, "{0}.ProcessQueueObject() - Sending Command", this.GetType().Name);
                Debug.PrintToConsole(eDebugLevel.Trace, command.ToString());

                // create the request
                WebRequest request = new WebRequest(Session.HostnameOrIPAddress, command.APIPath, 0, command.CommandRequestType, RequestAuthType.Basic, null, null, command.CombineHeaders(Session.HTTPHeaders), command.GetFormattedCommandString(), command.TimeoutEnabled, command.Timeout, true, false, false, Encoding.UTF8);

                // send the request and get the response
                WebResponse response = Session.Connection.SubmitRequest(request);

                Debug.PrintToConsole(eDebugLevel.Trace, "{0}.ProcessQueueObject() [ {1} ] : Received Response - {2}", this.GetType().Name, Name, response);

                // add the response to the queue to be processed
                if (command.ProcessResponse)
                {                    
                    if( OnResponseReceived != null && response != null )
                        OnResponseReceived(this, response);
                    else if( response == null )
                        Debug.PrintToConsole(eDebugLevel.Trace, "{0}.ProcessQueueObject() [ {1} ] :  NULL Response returned from REST client for command {2}", this.GetType().Name, Name, command.Path);
                }
            }
            catch (Crestron.SimplSharp.Net.Https.HttpsException httpsEx)
            {
                Debug.PrintToConsole(eDebugLevel.Notice,"{0}.ProcessQueueObject() [ {1} ] :  Network Error Processing Queue", this.GetType().Name, Name);

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