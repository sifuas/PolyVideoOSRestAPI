using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// crestron libraries
using Crestron.SimplSharp;
using RequestType = Crestron.SimplSharp.Net.Https.RequestType;

using PolyVideoOSRestAPI.Logging;

namespace PolyVideoOSRestAPI.Network
{

    /// <summary>
    /// Holds the data needed to make a request to a web service
    /// </summary>
    public class WebRequest : IDebuggable
    {
        // hostname or IP address to connect to
        public string Host { get; private set; }

        // the port on the host to connect to
        public int Port { get; private set; }

        // URL path to connect to
        public string Path { get; private set; }

        // type of request to make: GET, POST, ...
        public RequestType Type { get; private set; }

        // what type of authentication to use
        public RequestAuthType AuthenticationType { get; private set; }

        // username to use for authentication
        public string Username { get; private set; }

        // password to use for authentication
        public string Password { get; set; }

        // header values to add to request
        public IEnumerable<KeyValuePair<string, string>> Headers { get; private set; }

        // the content to send as a string
        public string Content { get; private set; }

        // the content to send as an array of bytes
        public byte[] ContentBytes { get; private set; }

        // set other parameters of the request
        public bool TimeoutEnabled { get; private set; }

        // the timeout value
        public int Timeout { get; private set; }

        // set the header to keep the connection alive
        public bool KeepAlive { get; private set; }

        // enable or disable peer verification for an HTTPS request
        public bool PeerVerification { get; private set; }

        // enable or disable host verification for an HTTPS request
        public bool HostVerification { get; private set; }

        // set the encoding type used for the message content
        public Encoding EncodingType { get; private set; }

        /// <summary>
        /// Create default WebRequest
        ///    TimeoutEnabled = false
        ///    PeerVerification = false
        ///    HostVerification = false
        ///    KeepAlive = true
        ///    EncodingType = Encoding.UTF8
        ///    AuthenticationType = RequestAuthType.NONE
        /// </summary>
        /// <param name="host">IP address or hostname to connect to</param>
        /// <param name="URLPath">The path to append to the host</param>
        /// <param name="port">The port to connect to</param>
        /// <param name="requestType">The type of request to send. GET, POST, ...</param>
        public WebRequest( string host, string URLPath, int port, RequestType requestType )
        {
            TimeoutEnabled = false;
            PeerVerification = false;
            HostVerification = false;
            KeepAlive = true;
            EncodingType = Encoding.UTF8;
            AuthenticationType = RequestAuthType.None;

            Host = host;
            Path = URLPath;
            Type = requestType;            
        }

        /// <summary>
        /// Create a WebRequest with string content
        /// </summary>
        /// <param name="host">IP address or hostname to connect to</param>
        /// <param name="URLPath">The path to append to the host</param>
        /// <param name="port">The port to connect to</param>
        /// <param name="requestType">The type of request to send. GET, POST, ...</param>
        /// <param name="authType">The type of authentication mechanism to use</param>
        /// <param name="userName">Username for authentication</param>
        /// <param name="password">Password for authentication</param>
        /// <param name="headers">The headers to add to the request</param>
        /// <param name="content">The request content as a string</param>
        /// <param name="timeoutEnabled">Enables the timeout period of the request</param>
        /// <param name="timeout">The timeout value to set</param>
        /// <param name="keepAlive">Set the HTTP keepalive state</param>
        /// <param name="peerVerification">Whether peer verification is used</param>
        /// <param name="hostVerification">Whether host verification is used</param>
        /// <param name="encoding">The type of encoding to use</param>
        public WebRequest(string host, string URLPath, int port, RequestType requestType, RequestAuthType authType, string userName, string password, IEnumerable<KeyValuePair<string, string>> headers,
            string content, bool timeoutEnabled, int timeout, bool keepAlive, bool peerVerification, bool hostVerification, Encoding encoding )
        {          
            Host = host;
            Path = URLPath;
            Port = port;
            Type = requestType;
            AuthenticationType = authType;
            Username = userName;
            Password = password;
            Headers = headers;
            Content = content;
            TimeoutEnabled = timeoutEnabled;
            Timeout = timeout;
            KeepAlive = keepAlive;
            PeerVerification = peerVerification;
            HostVerification = hostVerification;
            EncodingType = encoding;
        }


        /// <summary>
        /// Create a WebRequest with byte array content
        /// </summary>
        /// <param name="host">IP address or hostname to connect to</param>
        /// <param name="URLPath">The path to append to the host</param>
        /// <param name="port">The port to connect to</param>
        /// <param name="requestType">The type of request to send. GET, POST, ...</param>
        /// <param name="authType">The type of authentication mechanism to use</param>
        /// <param name="userName">Username for authentication</param>
        /// <param name="password">Password for authentication</param>
        /// <param name="headers">The headers to add to the request</param>
        /// <param name="content">The request content as a byte array</param>
        /// <param name="timeoutEnabled">Enables the timeout period of the request</param>
        /// <param name="timeout">The timeout value to set</param>
        /// <param name="keepAlive">Set the HTTP keepalive state</param>
        /// <param name="peerVerification">Whether peer verification is used</param>
        /// <param name="hostVerification">Whether host verification is used</param>
        /// <param name="encoding">The type of encoding to use</param>
        public WebRequest(string host, string URLPath, int port, RequestType requestType, RequestAuthType authType, string userName, string password, IEnumerable<KeyValuePair<string, string>> headers,
            byte[] content, bool timeoutEnabled, int timeout, bool keepAlive, bool peerVerification, bool hostVerification, Encoding encoding)
        {
            Host = host;
            Path = URLPath;
            Port = port;
            Type = requestType;
            AuthenticationType = authType;
            Username = userName;
            Password = password;
            Headers = headers;
            ContentBytes = content;
            TimeoutEnabled = timeoutEnabled;
            Timeout = timeout;
            KeepAlive = keepAlive;
            PeerVerification = peerVerification;
            HostVerification = hostVerification;
            EncodingType = encoding;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine("------------  Request -----------");
            str.AppendLine("Host = ");
            str.Append(Host);
            str.Append(", Path = ");
            str.Append(Path);
            str.Append(", Port = ");
            str.Append(Port);
            str.Append(", Type = ");
            str.Append(Type);
            str.Append(", Authentication Type = ");
            str.Append(AuthenticationType);
            str.AppendLine("Username = ");
            str.Append(Username);
            str.AppendLine("Password = ");
            str.Append(!String.IsNullOrEmpty(Password));
            str.AppendLine("TimeOutEnabled = ");
            str.Append(TimeoutEnabled);
            str.Append(", Timeout = ");
            str.Append(Timeout);
            str.Append(", KeepAlive = ");
            str.Append(KeepAlive);
            str.Append(", PeerVerification = ");
            str.Append(PeerVerification);
            str.Append(", HostVerification = ");
            str.Append(HostVerification);
            str.Append(", EncodingType = ");
            str.Append(EncodingType);

            if (!String.IsNullOrEmpty(Content))
            {
                str.AppendLine("Content:");
                str.AppendLine(Content);
            }

            if (ContentBytes != null && ContentBytes.Length > 0)
            {
                str.AppendLine("ContentBytes Exist");                
            }

            str.AppendLine("Headers = ");
            foreach (KeyValuePair<string, string> kvp in Headers)
            {
                str.AppendLine(kvp.Key + " = " + kvp.Value);
            }

            return str.ToString();
        }

        #region IDebuggable Members

        public void PrintDebugState()
        {
            CrestronConsole.PrintLine(this.ToString());
        }

        #endregion
    }
}