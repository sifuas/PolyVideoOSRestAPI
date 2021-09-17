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

namespace PolyVideoOSRestAPI.Timer
{
    /// <summary>
    /// Encapsulate timer state
    /// </summary>
    public sealed class TimerEventArgs : EventArgs
    {
        /// <summary>
        /// Returns the underlying TimeSpan
        /// </summary>
        public TimeSpan TimeSpanValue { get; private set; }

        /// <summary>
        /// Returns the amount of time as a percentage of the whole time.
        /// Value from 0 - 100
        /// </summary>
        public int PercentValue { get; private set; }

        /// <summary>
        /// Returns the amount of time as a serial string.
        /// </summary>
        public string SerialValue { get; private set; }

        /// <summary>
        /// Return the Hours component of the time.
        /// </summary>
        public int Hours { get { return TimeSpanValue.Hours; } }

        /// <summary>
        /// Return the Minutes component of the time.
        /// </summary>
        public int Minutes { get { return TimeSpanValue.Minutes; } }

        /// <summary>
        /// Return the Seconds component of the time.
        /// </summary>
        public int Seconds { get { return TimeSpanValue.Seconds; } }

        /// <summary>
        /// Return the Milliseconds component of the time.
        /// </summary>
        public int Milliseconds { get { return TimeSpanValue.Milliseconds; } }

        /// <summary>
        /// Return the Hours component of the time.
        /// </summary>
        public ushort HoursUShort { get { return (ushort)Hours; } }

        /// <summary>
        /// Return the Minutes component of the time.
        /// </summary>
        public ushort MinutesUShort { get { return (ushort)Minutes; } }

        /// <summary>
        /// Return the Seconds component of the time.
        /// </summary>
        public ushort SecondsUShort { get { return (ushort)Seconds; } }

        /// <summary>
        /// Return the Milliseconds component of the time.
        /// </summary>
        public ushort MillisecondsUShort { get { return (ushort)Milliseconds; } }

        /// <summary>
        ///  Return the Percent remaining value as a ushort
        ///  Value from 0 - 100
        /// </summary>
        public ushort PercentValueUshort { get { return (ushort)PercentValue; } }

        /// <summary>
        /// Default Constructor for Simpl+
        /// </summary>
        public TimerEventArgs() { }

        /// <summary>
        /// Create the event with the given timespan value
        /// </summary>
        /// <param name="timeSpan"></param>
        public TimerEventArgs(TimeSpan timeSpan, int percentageValue, string serialValue)
        {
            TimeSpanValue   = timeSpan;
            PercentValue    = percentageValue;
            SerialValue     = serialValue;
        }
    }
}