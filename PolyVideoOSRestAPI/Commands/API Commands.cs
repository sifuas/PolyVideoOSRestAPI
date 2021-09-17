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

// system includes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// crestron libraries
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net.Https;
using RequestType = Crestron.SimplSharp.Net.Https.RequestType;

//3rd party libraries
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using PolyVideoOSRestAPI.Logging;
using PolyVideoOSRestAPI;

namespace PolyVideoOSRestAPI.InputCommands
{
    /// <summary>
    /// Define the interface needed to transform the command object into text representing the selected type.
    /// Examples are returning the object as JSON, plain text, a URL query or a URL path
    /// </summary>
    internal interface APIInputCommand
    {
        string ToInputCommandString( eCommandInputFormatType inputType );        
    }

    // store information about specific commands needed to communicate with the API
    internal class APICommand : IComparable, IDisposable, IDebuggable
    {
        /// <summary>
        /// Define the command priorities that can be used if the command is stored in priority order.
        /// </summary>
        internal enum VideoOSAPICommandPriority
        {
            IMMEDIATE = 1,
            STANDARD  = 10,
            POLLING   = 100
        }

        // the REST path for the command
        private string _path = "";
        public string Path { get { return _path; } }
        
        // returns the full api path for the command
        public string APIPath { get { return APIGlobal.CreateFullRESTAPIPath(Path); } }

        // the type of the HTTP request needed for this command
        public RequestType CommandRequestType { get; set; }

        // the define priority for this command
        public VideoOSAPICommandPriority Priority { get; set; }

        // The format type this command should use the input data
        public eCommandInputFormatType Format { get; private set; }

        // The input object used to create the value of the input to send
        public APIInputCommand CommandObject { get; private set; }

        // headers needed for this command
        public Dictionary<string, string> Headers { get; private set; }

        // flag indicating whether this command should process the response
        public bool ProcessResponse { get; private set; }

        // should this command enable a timeout when sent?
        public bool TimeoutEnabled { get; set; }

        // if the timeout is enable, set the time
        public int Timeout { get; set; }

        /// <summary>
        /// Creates a default command with no formatting, standard priority and GET request type
        /// </summary>
        /// <param name="path"></param>
        public APICommand(string path)
        {
            _path       = path;
            Format      = eCommandInputFormatType.NONE; 
            Priority    = VideoOSAPICommandPriority.STANDARD;
            CommandRequestType = RequestType.Get;
            ProcessResponse = true;
            TimeoutEnabled = true;
            Timeout = 60;
        }

        public APICommand(string path, RequestType commandRequestType, bool processResponse )             
        {
            _path = path;
            Format = eCommandInputFormatType.NONE;
            Priority = VideoOSAPICommandPriority.STANDARD;    
            CommandRequestType = commandRequestType;
            ProcessResponse = processResponse;
            TimeoutEnabled = true;
            Timeout = 60;
        }

        public APICommand(string path, eCommandInputFormatType format)
        {
            _path       = path;
            Format      = format;
            Priority    = VideoOSAPICommandPriority.STANDARD;
            CommandRequestType = RequestType.Get;
            ProcessResponse = true;
            TimeoutEnabled = true;
            Timeout = 60;
        }

        public APICommand(string path, eCommandInputFormatType format, APIInputCommand commandObject, bool processResponse)
        {
            _path = path;
            Format = format;            
            CommandObject = commandObject;
            ProcessResponse = processResponse;

            CommandRequestType = RequestType.Get;
            TimeoutEnabled = true;
            Timeout = 60;
            Priority = VideoOSAPICommandPriority.STANDARD;
        }

        public APICommand(string path, eCommandInputFormatType format, APIInputCommand commandObject, Dictionary<string, string> headers, bool processResponse)
        {
            _path = path;
            Format = format;
            CommandObject = commandObject;
            ProcessResponse = processResponse;
            Headers = headers;

            CommandRequestType = RequestType.Get;
            TimeoutEnabled = true;
            Timeout = 60;
            Priority = VideoOSAPICommandPriority.STANDARD;         
        }

        public APICommand(string path, eCommandInputFormatType format, APIInputCommand commandObject, Dictionary<string, string> headers, RequestType commandRequestType, bool processResponse, bool timeoutEnabled, int timeout )
        {
            _path = path;
            Format = format;
            CommandObject = commandObject;
            ProcessResponse = processResponse;
            Headers = headers;
            CommandRequestType = commandRequestType;
            TimeoutEnabled = timeoutEnabled;
            Timeout = timeout;

            Priority = VideoOSAPICommandPriority.STANDARD;  
        }

        /// <summary>
        /// Combines the headers from this command with any additional headers.
        /// The headers in this command will overwrite any additional headers with the same name.
        /// </summary>
        /// <param name="additionalHeaders">Dictionary of any additional headers to add to the command</param>
        /// <returns>The dictionary containing all of the headers to use for the command.</returns>
        internal Dictionary<string, string> CombineHeaders( Dictionary<string,string> additionalHeaders )
        {
            Dictionary<string, string> returnHeaders = new Dictionary<string, string>();
            bool sessionHeadersSet = ( additionalHeaders != null );

            // check the session headers
            if (sessionHeadersSet)
            {
                foreach (KeyValuePair<string, string> kvp in additionalHeaders)
                {
                    returnHeaders.Add(kvp.Key, kvp.Value);
                }
            }

            // add any command specific headers and override any session headers
            if (Headers != null)
            {
                foreach( KeyValuePair<string,string> kvp in Headers )
                {
                    if (sessionHeadersSet)
                    {
                        if (returnHeaders.ContainsKey(kvp.Key))
                            returnHeaders.Remove(kvp.Key);
                    }

                    returnHeaders.Add(kvp.Key, kvp.Value);
                }
            }

            return returnHeaders;
        }

        /// <summary>
        /// Returns the Formatted command string needed for this command.
        /// The formatted command string will depend on the type of CommandObject, RequestType and the Format expected
        /// </summary>
        /// <returns></returns>
        internal string GetFormattedCommandString()
        {
            return ((CommandObject == null) ? "" : CommandObject.ToInputCommandString(Format));
        }

        /// <summary>
        /// Output the command details including the APIPath and formatted command string for this command.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format( "Command : {0}, {1}", APIPath, GetFormattedCommandString());
        }

        /// <summary>
        /// Implement the IComparable Interface.
        /// Compares this command to others in relation to its priority.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
           if( obj == null )
               return 1;

            APICommand otherCommand = obj as APICommand;

            if( otherCommand != null )
                return ( this.Priority.CompareTo( otherCommand.Priority ) );
            else
                throw new ArgumentException( "Object is not a VideoOSAPICommand");
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (Headers != null)
                Headers.Clear();
            Headers = null;
            CommandObject = null;
        }

        #endregion

        #region IDebuggable Members

        public void PrintDebugState()
        {
            CrestronConsole.PrintLine(this.ToString());
        }

        #endregion
    }

}