using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace MEI.Integration.PolyVideoOSRestAPI.Queue
{
    /// <summary>
    /// Define the interface for a generic message that can be processed by a queue
    /// </summary>
    public interface IQueueMessage
    {
        /// <summary>
        /// Dispatch the message using the class specific mechanism.
        /// This could be an Action associated with the message, sending the message to a COM port, etc...
        /// </summary>
        void Dispatch();
    }
}