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

using MEI.Integration.PolyVideoOSRestAPI;

namespace MEI.Integration.PolyVideoOSRestAPI.InputCommands
{
    /// <summary>
    /// Define the interface needed to transform the command object into text representing the selected type.
    /// Examples are returning the object as JSON, plain text, a URL query or a URL path
    /// </summary>
    public interface VideoOSInputCommandObject
    {
        string ToInputCommandString( VideoOSAPICommandInputFormatType inputType );        
    }

    // store information about specific commands needed to communicate with the API
    public class VideoOSAPICommand
    {
        // the REST path for the command
        private string _path = "";
        public string Path { get { return _path; } }
        
        // returns the full api path for the command
        public string APIPath { get { return VideoOSGlobal.CreateFullRESTAPIPath(Path); } }

        public RequestType CommandRequestType { get; set; }

        // The format type this command should use the input data
        public VideoOSAPICommandInputFormatType Format { get; private set; }

        // The input object used to create the value of the input to send
        public VideoOSInputCommandObject CommandObject { get; set; }

        // headers needed for this command
        public Dictionary<string, string> Headers { get; set; }

        public VideoOSAPICommand(string path)
        {
            _path = path;
            Format = VideoOSAPICommandInputFormatType.NONE;            
        }

        public VideoOSAPICommand(string path, VideoOSAPICommandInputFormatType format )
        {
            _path = path;
            Format = format;
        }

        public VideoOSAPICommand(string path, VideoOSAPICommandInputFormatType format, VideoOSInputCommandObject commandObject )
            : this(path, format)
        {
            CommandObject = commandObject;            
        }

        public VideoOSAPICommand(string path, VideoOSAPICommandInputFormatType format, VideoOSInputCommandObject commandObject, Dictionary<string,string> headers )
            : this(path, format, commandObject)
        {
            Headers = headers;            
        }

        public VideoOSAPICommand(string path, VideoOSAPICommandInputFormatType format, VideoOSInputCommandObject commandObject, Dictionary<string,string> headers, RequestType commandRequestType )
            : this(path, format, commandObject )
        {
            Headers = headers;
            CommandRequestType = commandRequestType;
        }

        public string GetFormattedCommandString()
        {
            return ((CommandObject == null) ? "" : CommandObject.ToInputCommandString(Format));
        }

        public override string ToString()
        {
            return String.Format( "Command : {0}, {1}", APIPath, GetFormattedCommandString());
        }
    }

}