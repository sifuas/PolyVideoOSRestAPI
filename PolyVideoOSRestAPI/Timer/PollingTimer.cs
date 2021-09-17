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

using PolyVideoOSRestAPI.Logging;

namespace PolyVideoOSRestAPI.Timer
{

    /// <summary>
    /// Class to encapsulate the logic and timing for sending polling commands
    /// </summary>
    public class PollingTimer : IDisposable
    {
        // the timer to use for polling
        private CTimer pollingTimer;

        // the delegate to handle the callback
        public CTimerCallbackFunction TimerCallback;

        // store the timer values for the timer
        public long TimerPeriodInMs { get; set; }
        public long TimerRepeatPeriodInMs { get; set; }

        public long TimerExtendedPeriodInMs { get; set; }
        public long TimerExtendedRepeatPeriodInMs { get; set; }

        // assign a name to the timer
        public string Name { get; set; }

        // has the object been disposed
        public bool Disposed { get; set; }

        /// <summary>
        /// Create the timer with the given times.
        /// This will set the extended times to match the standard times.
        /// </summary>
        /// <param name="timerPeriodInMs"></param>
        /// <param name="timerRepeatPeriodInMs"></param>
        public PollingTimer(long timerPeriodInMs, long timerRepeatPeriodInMs, string name )
        {
            TimerPeriodInMs                 = timerPeriodInMs;
            TimerRepeatPeriodInMs           = TimerRepeatPeriodInMs;
            TimerExtendedPeriodInMs         = timerPeriodInMs;
            TimerExtendedRepeatPeriodInMs   = TimerRepeatPeriodInMs;

            Name = name;
        }

        /// <summary>
        /// Create the timer with the given times.
        /// </summary>
        /// <param name="timerPeriodInMs"></param>
        /// <param name="timerRepeatPeriodInMs"></param>
        /// <param name="timerExtendedPeriodInMs"></param>
        /// <param name="timerExtendedRepeatPeriodInMs"></param>
        public PollingTimer(long timerPeriodInMs, long timerRepeatPeriodInMs, long timerExtendedPeriodInMs, long timerExtendedRepeatPeriodInMs, string name )            
        {
            TimerPeriodInMs                 = timerPeriodInMs;
            TimerRepeatPeriodInMs           = TimerRepeatPeriodInMs;
            TimerExtendedPeriodInMs         = timerExtendedPeriodInMs;
            TimerExtendedRepeatPeriodInMs   = timerExtendedRepeatPeriodInMs;

            Name = name;
        }

        /// <summary>
        /// Start the timer
        /// </summary>
        public void Start()
        {
            Debug.PrintToConsole(eDebugLevel.Trace, "Timer [{0}] Start(): {1}, {2}, {3}, {4}", Name, TimerPeriodInMs, TimerPeriodInMs, TimerExtendedPeriodInMs, TimerExtendedRepeatPeriodInMs);

            if (Disposed)
                return;

            if (pollingTimer == null)
                pollingTimer = new CTimer(TimerCallback, this, TimerPeriodInMs, TimerRepeatPeriodInMs);
            else
                pollingTimer.Reset(TimerPeriodInMs, TimerRepeatPeriodInMs);
        }

        /// <summary>
        /// Start the timer with the extended time period
        /// </summary>
        public void StartExtended()
        {
            Debug.PrintToConsole(eDebugLevel.Trace, "Timer [{0}] StartExtended(): {1}, {2}, {3}, {4}", Name, TimerPeriodInMs, TimerPeriodInMs, TimerExtendedPeriodInMs, TimerExtendedRepeatPeriodInMs);


            if (Disposed)
                return;

            if (pollingTimer == null)
                pollingTimer = new CTimer(TimerCallback, this, TimerExtendedPeriodInMs, TimerExtendedRepeatPeriodInMs);
            else
                pollingTimer.Reset(TimerExtendedPeriodInMs, TimerExtendedRepeatPeriodInMs);
        }

        /// <summary>
        /// Reset the timer with the new timer values
        /// </summary>
        /// <param name="timerPeriodInMs"></param>
        /// <param name="timerRepeatPeriodInMs"></param>
        public void Reset(long timerPeriodInMs, long timerRepeatPeriodInMs)
        {
            Debug.PrintToConsole(eDebugLevel.Trace, "Timer [{0}] Reset(): {1}, {2}, {3}, {4}", Name, TimerPeriodInMs, TimerPeriodInMs, TimerExtendedPeriodInMs, TimerExtendedRepeatPeriodInMs);

            if (Disposed)
                return;

            TimerPeriodInMs         = timerPeriodInMs;
            TimerRepeatPeriodInMs   = timerRepeatPeriodInMs;

            Start();
        }

        /// <summary>
        /// Reset the timer with the new extended timer values
        /// </summary>
        /// <param name="timerExtendedPeriodInMs"></param>
        /// <param name="timerExtendedRepeatPeriodInMs"></param>
        public void ResetExtended(long timerExtendedPeriodInMs, long timerExtendedRepeatPeriodInMs)
        {
            Debug.PrintToConsole(eDebugLevel.Trace, "Timer [{0}] ResetExtended(): {1}, {2}, {3}, {4}", Name, TimerPeriodInMs, TimerPeriodInMs, TimerExtendedPeriodInMs, TimerExtendedRepeatPeriodInMs);

            if (Disposed)
                return;

            TimerExtendedPeriodInMs         = timerExtendedPeriodInMs;
            TimerExtendedRepeatPeriodInMs   = timerExtendedRepeatPeriodInMs;

            StartExtended();
        }

        /// <summary>
        /// Stop the timer
        /// </summary>
        public void Stop()
        {
            Debug.PrintToConsole(eDebugLevel.Trace, "Timer [{0}] Stop(): {1}, {2}, {3}, {4}", Name, TimerPeriodInMs, TimerPeriodInMs, TimerExtendedPeriodInMs, TimerExtendedRepeatPeriodInMs);

            if (Disposed)
                return;

            if (pollingTimer != null)
                pollingTimer.Stop();
        }

        /// <summary>
        /// Dispose method from IDisposable Interface.
        /// Stops and disposes the timer and cleans up all other resources.
        /// </summary>
        public void Dispose()
        {
            Debug.PrintToConsole(eDebugLevel.Trace, "Timer [{0}] Dispose(): {1}, {2}, {3}, {4}", Name, TimerPeriodInMs, TimerPeriodInMs, TimerExtendedPeriodInMs, TimerExtendedRepeatPeriodInMs);

            Disposed = true;

            if ((pollingTimer != null) && !pollingTimer.Disposed)
            {
                pollingTimer.Stop();
                pollingTimer.Dispose();
            }

            TimerCallback = null;
        }
       
    }
}