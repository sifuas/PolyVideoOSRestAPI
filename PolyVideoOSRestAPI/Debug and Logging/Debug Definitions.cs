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
using Crestron.SimplSharp.CrestronLogger;

namespace PolyVideoOSRestAPI.Logging
{
    /// <summary>
    /// Represents a device that can be debugged and provides functions to help with debugging.
    /// </summary>
    public interface IDebuggable
    {
        void PrintDebugState();
    }

    /// <summary>
    /// Encapsulate logic for debugging
    /// </summary>
    public static class ProjectDebug
    {
        public static eDebugLevel DebugLevel { get; set; }

        static ProjectDebug()
        {            
            DebugLevel = eDebugLevel.Error;
        }

        /// <summary>
        /// Check the given debug level against the currently set debugging system level.
        /// If the given level is greater than the system debug level then return true.
        /// </summary>
        /// <param name="level"></param>
        /// <returns>True if level > DebugLevel, otherwise false.</returns>
        public static bool CheckDebugLevel(eDebugLevel level)
        {
            return (level >= DebugLevel);
        }

        /// <summary>
        /// Set the debug level
        /// </summary>
        /// <param name="newLevel"></param>
        public static void SetDebugLevel(eDebugLevel newLevel)
        {
            DebugLevel = newLevel;
        }

        /// <summary>
        /// Print the message to the console
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        public static void PrintToConsole(eDebugLevel level, string msg)
        {
            if (CheckDebugLevel( level ))
                CrestronConsole.PrintLine( msg );
        }

        public static void PrintToConsole(eDebugLevel level, string msg, params object[] args)
        {
            PrintToConsole(level,String.Format(msg, args));
        }
               
        /// <summary>
        /// Write the message to the log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        public static void PrintToLog(eDebugLevel level, string msg)
        {
            //ErrorLog.Notice(String.Format("Log Level = {0}, Sent Level = {1}, Message = {2}", DebugLevel, level, msg));

            if (CheckDebugLevel(level))
            {
                switch (level)
                {
                    case (eDebugLevel.Trace):                        
                    case (eDebugLevel.Notice):
                        ErrorLog.Notice(msg);
                        break;
                    case (eDebugLevel.Error):
                        ErrorLog.Notice(msg);
                        break;
                    case (eDebugLevel.Warning):
                        ErrorLog.Warn(msg);
                        break;
                }
            }
        }

        public static void PrintToLog(eDebugLevel level, string msg, params object[] args)
        {
            PrintToLog(level, String.Format(msg, args));
        }

        /// <summary>
        /// Print to both the console and the log
        /// </summary>
        /// <param name="level"></param>
        /// <param name="msg"></param>
        public static void PrintToConsoleAndLog(eDebugLevel level, string msg)
        {
            //ErrorLog.Notice("PrintToConsoleAndLog( ) - Current Level " + DebugLevel + " Provided Level " + level);
            //ErrorLog.Notice("Message = " + msg);

            if (CheckDebugLevel( level ))
            {
                PrintToConsole(level, msg);
                PrintToLog(level, msg);
            }
        }

        public static void PrintToConsoleAndLog(eDebugLevel level, string msg, params object[] args)
        {
            PrintToConsoleAndLog(level, String.Format(msg, args));
        }

        /// <summary>
        /// Print the given exception and message to the log
        /// </summary>
        /// <param name="e"></param>
        /// <param name="msg"></param>
        public static void PrintExceptionToLog(eDebugLevel level, Exception e, string msg)
        {
            ErrorLog.Exception(msg, e);
        }

        public static void PrintExeptionToLog(eDebugLevel level, Exception e, string msg, params object[] args)
        {
            PrintExceptionToLog(level, e, string.Format(msg, args));
        }

        /// <summary>
        /// Print the given exception and message to the console
        /// </summary>
        /// <param name="e"></param>
        /// <param name="msg"></param>
        public static void PrintExceptionToConsole(eDebugLevel level, Exception e, string msg)
        {
            try
            {

                if (CheckDebugLevel(level))
                {
                    if (msg != null)
                        CrestronConsole.PrintLine(msg);

                    CrestronConsole.PrintLine("Exception ( {0} ) : {1}", e.GetType().ToString(), e.Message);
                    CrestronConsole.PrintLine("Stacktrace : " + e.StackTrace);

                    if (e.InnerException != null)
                        CrestronConsole.PrintLine("Inner Exception : " + e.InnerException);
                }
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("ProjectDebug.PrintExceptionToConsole() - Error handling error {0} - {1}", ex.Message, ex.StackTrace);
            }
        }

        public static void PrintExceptionToConsole(eDebugLevel level, Exception e, string msg, params object[] args)
        {
            PrintExceptionToConsole(level, e, String.Format(msg, args));
        }

        public static void PrintExceptionToConsoleAndLog(eDebugLevel level, Exception e, string msg)
        {
            PrintExceptionToConsole(level, e, msg);
            PrintExceptionToLog(level, e, msg);
        }

        public static void PrintExceptionToConsoelAndLog(eDebugLevel level, Exception e, string msg, params object[] args)
        {
            PrintExceptionToConsoelAndLog(level, e, String.Format(msg, args));
        }
    }


    /// <summary>
    /// Define debug levels
    /// </summary>
    public enum eDebugLevel : ushort
    {
        Trace,
        Notice,
        Warning,
        Error,        
    }
}