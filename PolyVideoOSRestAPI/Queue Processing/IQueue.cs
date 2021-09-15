using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace PolyVideoOSRestAPI.Queue
{
    /// <summary>
    /// Interface to define basic Queueing operations
    /// </summary>
    public interface IQueue<T> : IDisposable
    {
        // add the item to the queue
        void Enqueue(T queueItem);

        // return the count of the number of items in the Queue
        int Count { get; }       
    }
}