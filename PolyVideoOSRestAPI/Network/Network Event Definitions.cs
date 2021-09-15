using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Crestron.SimplSharp;

namespace MEI.Integration.PolyVideoOSRestAPI.Network
{
    /// <summary>
    /// Encapsulate messages receive from the network along with information about where they were received from.
    /// </summary>
    public class NetworkDataMessageReceivedArgs : EventArgs 
    {
        // hostname or IP address of the host the message was received from
        public string IPOrHostname { get; private set; }

        // port on the host the message was received from
        public ushort Port { get; private set; }

        // message receive from the host
        public string Message { get; private set; }

        // message in byte array received from the host
        public byte[] MessageBytes { get; private set; }

        // simpl+
        public NetworkDataMessageReceivedArgs(){}

        /// <summary>
        /// Create a message event with the host information and message data
        /// </summary>
        /// <param name="ipOrHostName"></param>
        /// <param name="port"></param>
        /// <param name="message"></param>
        /// <param name="messageBytes"></param>
        public NetworkDataMessageReceivedArgs(string ipOrHostName, ushort port, string message, byte[] messageBytes)
        {
            IPOrHostname = ipOrHostName;
            Port = port;
            Message = message;
            MessageBytes = messageBytes;
        }

    }
}