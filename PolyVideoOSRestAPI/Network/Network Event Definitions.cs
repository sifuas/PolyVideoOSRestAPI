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

using Crestron.SimplSharp;

namespace PolyVideoOSRestAPI.Network
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