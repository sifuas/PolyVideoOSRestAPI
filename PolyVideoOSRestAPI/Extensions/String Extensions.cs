using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace PolyVideoOSRestAPI
{
    public static class Extensions
    {
        /// <summary>
        /// Returns a null string if input is an empty string.
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Null if empty, otherwise the input itself</returns>
        public static string EmptyIfNull(this string input)
        {
            return input == null ? "" : input;
        }
    }
}