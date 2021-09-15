using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

// crestron libraries
using Crestron.SimplSharp;

using PolyVideoOSRestAPI.Logging;

namespace PolyVideoOSRestAPI.Queue
{
    /// <summary>
    /// Encapsulate the queue processing logic
    /// </summary>
    public abstract class ProcessingQueueThreadBase<T> : IQueue<T>, IDebuggable
    {
        public string Key { get; private set; }

        public string Name { get; private set; }

        // IQueue.Count
        public int Count { get { return ( ( _disposed || ( _processingQueue == null ) ) ? 0 : _processingQueue.Count ); } }

        // the lock for thread        
        private CEvent _queueWaitHandle = new CEvent();

        // the queue of objects to process
        private CrestronQueue<T> _processingQueue;

        // has the object been disposed
        private bool _disposed = false;
        public bool IsDisposed { get { return _disposed; } }

        /// <summary>
        /// Create the queue processing thread with the given name
        /// </summary>
        /// <param name="queueName">The name to identify the queue</param>
        public ProcessingQueueThreadBase(string queueName)
            : this(queueName, 100)
        {

        }

        /// <summary>
        /// Create the queue processing thread with the given name and size
        /// </summary>
        /// <param name="queueName">The name to identify the queue</param>
        /// <param name="size">The size of the initial queue</param>
        public ProcessingQueueThreadBase( string queueName, int size ) 
        {            
            Name = queueName;         
        }

        /// <summary>
        /// Overriden function to handle the specific actions that should be performed on the object from the queue.
        /// If the processing should happen more quickly then the derived class should implement threading to handle the queue processing.
        /// </summary>
        /// <param name="queueObject"></param>
        public abstract void ProcessQueueObject( T queueObject );

        /// <summary>
        /// Add a new object to the queue to be processed.
        /// </summary>
        /// <param name="queueObject"></param>
        public void Enqueue(T queueObject)
        {
            if (IsDisposed)
            {
                Debug.PrintToConsole(eDebugLevel.Trace, "{0}<{1}>.ProcessQueue() [ {2} ] :  Can't Enqueue new Object, Queue already disposed.", this.GetType().Name, typeof(T).FullName, Name);
                //CrestronConsole.PrintLine( "{0}<{1}>.ProcessQueue() [ {2} ] :  Can't Enqueue new Object, Queue already disposed.", this.GetType().Name, typeof(T).FullName, Name);            
                return;
            }

            if (queueObject == null)
                throw new ArgumentException("Cannot add NULL object to the Queue");

            bool firstTimeEnqueue = ( _processingQueue == null );

            if (firstTimeEnqueue)
            {                
                _processingQueue = new CrestronQueue<T>();                
            }

            Debug.PrintToConsole(eDebugLevel.Trace, "{0}<{1}>.ProcessQueue() [ {2} ] :  Enqueue new Object.", this.GetType().Name, typeof(T).FullName, Name);            

            // enqueue the object to process
            _processingQueue.Enqueue(queueObject);

            if (firstTimeEnqueue)
            {
                Debug.PrintToConsole(eDebugLevel.Trace, "{0}<{1}>.ProcessQueue() [ {2} ] :  Starting Dequeue Processing Thread", this.GetType().Name, typeof(T).FullName, Name);

                CrestronInvoke.BeginInvoke(thread => ProcessQueue());
            }

            Debug.PrintToConsole(eDebugLevel.Trace, "{0}<{1}>.ProcessQueue() [ {2} ] :  Signaling Dequeue Thread to Wake Up", this.GetType().Name, typeof(T).FullName, Name);

            // signal to the potentially waiting thread to wake up and process the object
            _queueWaitHandle.Set();
            
        }

        /// <summary>
        /// Clear any objects events from the queue
        /// </summary>
        public void Clear()
        {
            if (IsDisposed)
            {
                Debug.PrintToConsole(eDebugLevel.Trace, "{0}<{1}>.ProcessQueue() [ {2} ] :  Can't Clear Queue, Queue already disposed.", this.GetType().Name, typeof(T).FullName, Name);
                return;
            }
            
            //Debug.PrintToConsole(eDebugLevel.Trace, "{0}<{1}>.ProcessQueue() [ {2} ] :  Clear() Count = {3}", this.GetType().Name, typeof(T).FullName, Name, _processingQueue.Count);
            if( _processingQueue != null )
                _processingQueue.Clear();
        }

        /// <summary>
        /// Process the queue by passing the objects of type T to the overriden method of the base class for them to be processed.
        /// </summary>
        private void ProcessQueue()
        {            
            try
            {
                Debug.PrintToConsole(eDebugLevel.Trace, "{0}<{1}>.ProcessQueue() [ {2} ] :  Starting", this.GetType().Name, typeof(T).FullName, Name);
                //CrestronConsole.PrintLine( "{0}<{1}>.ProcessQueue() [ {2} ] :  Starting", this.GetType().Name, typeof(T).FullName, Name);
                //ErrorLog.Notice("{0}<{1}>.ProcessQueue() [ {2} ] :  Starting", this.GetType().Name, typeof(T).FullName, Name);  

                while (true)
                {
                        T queueObject = default(T);

                        // check if the queue should exit
                        if ( IsDisposed == true)
                        {
                            Debug.PrintToConsoleAndLog(eDebugLevel.Trace, "{0}<{1}>.ProcessQueue() [ {2} ] :  Exiting ProcessQueue().  Disposed = {4}", this.GetType().Name, typeof(T).FullName, Name, _disposed);
                            //CrestronConsole.PrintLine( "{0}<{1}>.ProcessQueue() [ {2} ] :  Exiting ProcessQueue().  Disposed = {4}", this.GetType().Name, typeof(T).FullName, Name, _disposed);
                            //ErrorLog.Notice("{0}<{1}>.ProcessQueue() [ {2} ] :  Exiting ProcessQueue().  Disposed = {4}", this.GetType().Name, typeof(T).FullName, Name, _disposed);
                            return;
                        }

                        // verify that there is an object to process
                        if (_processingQueue.Count > 0)
                        {
                            // remove the object from the queue
                            queueObject = _processingQueue.Dequeue();
                        }
                        else
                        {
                            // block until an object is added
                            _queueWaitHandle.Wait();
                            //CrestronConsole.PrintLine("{0}<{1}>.ProcessQueue() [ {2} ] :  Wake Up For Enqueue", this.GetType().Name, typeof(T).FullName, Name);
                        }

                        // process the object retrieved from the queue
                        if (queueObject != null)
                        {
                            try
                            {
                                ProcessQueueObject(queueObject);
                            }
                            catch (Exception ex)
                            {
                                CrestronConsole.PrintLine( "{0}<{1}>.ProcessQueue() [ {2} ] : Error = {3}", this.GetType().Name, typeof(T).Name, Name, ex.Message);
                                CrestronConsole.PrintLine( "{0}<{1}>.ProcessQueue() [ {2} ] : Error = {3}", this.GetType().Name, typeof(T).Name, Name, ex.StackTrace);
                                ErrorLog.Error( "{0}<{1}>.ProcessQueue() [ {2} ] : Error = {3}", this.GetType().Name, typeof(T).Name, Name, ex.Message);
                                ErrorLog.Error( "{0}<{1}>.ProcessQueue() [ {2} ] : Error = {3}", this.GetType().Name, typeof(T).Name, Name, ex.StackTrace);
                            }
                        }

                }
            }
            finally
            {
                Debug.PrintToConsole(eDebugLevel.Trace, "{0}<{1}>.ProcessQueue() [ {2} ] : Exiting", this.GetType().Name, typeof(T).Name, Name);
                //ErrorLog.Notice("{0}<{1}>.ProcessQueue() [ {2} ] : Exiting", this.GetType().Name, typeof(T).Name, Name);
            }
        }

        /// <summary>
        /// IDisposable implementation
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;
                                                                                        
            if (disposing)
            {
                if (_processingQueue != null)
                {
                    _processingQueue.Clear();
                    _processingQueue.Enqueue(default(T));
                    _processingQueue.Dispose();
                }

                _queueWaitHandle.Close();
                _processingQueue = null;
            }                
        }

        #region IDebuggable Members

        public virtual void PrintDebugState()
        {
            CrestronConsole.PrintLine("{0} {1} State, Disposed = {2}", this.GetType().Name, Name, _disposed);

            if (!_disposed && _processingQueue != null)
            {
                CrestronConsole.PrintLine("Queue Count = {0}", _processingQueue.Count);

                if( typeof(IDebuggable).IsAssignableFrom(typeof(T)))
                {
                    T[] objects = new T[_processingQueue.Count];
                    _processingQueue.CopyTo( objects, 0 );

                    foreach (T obj in objects)
                    {
                        if( obj != null )
                            ((IDebuggable)obj).PrintDebugState();
                    }
                }
            }
        }

        #endregion
    }
}