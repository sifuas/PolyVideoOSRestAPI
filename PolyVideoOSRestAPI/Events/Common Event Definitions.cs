﻿/*
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

namespace PolyVideoOSRestAPI.Events
{

    /// <summary>
    /// Generic Event Argument indicating a state change in the program 
    /// </summary>
    public abstract class AChangeEventArgsBase<T> : EventArgs
    {
        public ushort EventType { get; set; }

        public T Value { get; set; }

        public T LastValue { get; set; }

        /// <summary>
        /// Default constructor needed for Simpl+
        /// </summary>
        public AChangeEventArgsBase() 
        {
            Value = default(T);
            LastValue = default(T);
        }

        public AChangeEventArgsBase( T value )
        {
            Value = value;
            LastValue = default(T);
        }

        /// <summary>
        /// Sets the type of the event.
        /// </summary>
        /// <param name="type">UShort indicating custom event type</param>
        public AChangeEventArgsBase(T value, ushort type)
            : this()
        {
            this.Value = value;
            this.EventType = type;
            LastValue = default(T);
        }

        /// <summary>
        /// Sets the type of the event.
        /// </summary>
        /// <param name="type">UShort indicating custom event type</param>
        public AChangeEventArgsBase( T value, T lastValue, ushort type ) : this()
        {
            this.Value = value;
            this.EventType = type;
            LastValue = lastValue;
        }
    }

    /// <summary>
    /// Event argument indicating the change in a boolean state
    /// </summary>
    public class BooleanChangeEventArgs : AChangeEventArgsBase<bool>
    {

        // return the boolean value as a UShort to be compatible with Simpl+
        public ushort UShortValue
        {
            get
            {
                return ((ushort)((Value == true) ? 1 : 0));
            }
        }

        /// <summary>
        /// Default constructor needed for Simpl+
        /// </summary>
        public BooleanChangeEventArgs() { }

        /// <summary>
        /// Create an event args with the given boolean value and custom type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public BooleanChangeEventArgs(bool value, ushort type)
            : base(value, type)
        {            

        }

    }

    /// <summary>
    /// Event arguments indicating the change in a numeric value
    /// </summary>
    public class UShortChangeEventArgs : AChangeEventArgsBase<ushort>
    {
        /// <summary>
        /// Default constructor needed for Simpl+
        /// </summary>
        public UShortChangeEventArgs() { }

        /// <summary>
        /// Create an event arrgs with the given ushort value and custom type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public UShortChangeEventArgs(ushort value, ushort type)
            : base(value, type)
        {
            
        }
    }

    /// <summary>
    /// Event arguments indicating the change in a string value
    /// </summary>
    public class StringChangeEventArgs : AChangeEventArgsBase<string>
    {
        /// <summary>
        /// Default constructor needed for Simpl+
        /// </summary>
        public StringChangeEventArgs() { }

        /// <summary>
        /// Create an event arrgs with the given String value and custom type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public StringChangeEventArgs(String value, ushort type)
            : base(value, type)
        {            
        }
    }

    /// <summary>
    /// Event arguments indicating the change in an error state
    /// </summary>
    public class ErrorChangeEventArgs : AChangeEventArgsBase<string>
    {
        public bool IsError { get; private set; }

        public ushort UShortIsError
        {
            get { return (ushort)(IsError ? 1 : 0); }

        }

        /// <summary>
        /// Default constructor needed for Simpl+
        /// </summary>
        public ErrorChangeEventArgs() { }

        /// <summary>
        /// Create an event arrgs with the given String value and custom type
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public ErrorChangeEventArgs(bool isError, String value )
            : base(value)
        {
            IsError = isError;
        }
    }
}