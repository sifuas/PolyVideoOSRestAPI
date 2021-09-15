using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// simpl# Library
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net.Http;

// Core Library
using PolyVideoOSRestAPI.Logging;

namespace PolyVideoOSRestAPI.Network
{
    /// <summary>
    /// Misc. functions to help with network communications
    /// </summary>
    public static class NetworkHelperFunctions
    {
        public static bool HTTPStatusIsClientError(HttpStatusCode code)
        {
            return ((code >= HttpStatusCode.BadRequest) && (code <= HttpStatusCode.UpgradeRequired));
        }

        public static bool HTTPStatusIsServerError(HttpStatusCode code)
        {
            return ((code >= HttpStatusCode.InternalServerError) && (code <= HttpStatusCode.HttpVersionNotSupported));
        }

        /// <summary>
        /// Convert the given byte array into a string of hex digits
        /// </summary>
        /// <param name="messageBytes"></param>
        /// <returns></returns>
        public static string ConvertToHex(byte[] messageBytes)
        {
            return String.Concat(messageBytes.Select(hexByte => string.Format(@"{0:X2}", (int)hexByte)).ToArray());
        }

        /// <summary>
        /// Convert the given string into a string of hex digits
        /// </summary>
        /// <param name="messageString"></param>
        /// <returns></returns>
        public static string ConvertToHex(string messageString)
        {
            byte[] stringBytes = Encoding.GetEncoding(Global.DefaultEncodingCodePage).GetBytes(messageString);
            return ConvertToHex(stringBytes);
        }



        /// <summary>
        /// Convert the given string to Base 64 Encoding based on the default default encoding of ISO-8859-1
        /// </summary>
        /// <param name="stringToEncode">The string to convert to Base64</param>
        /// <returns></returns>
        public static string ConvertToDefaultBase64Encoding(string stringToEncode)
        {
            return ConvertToBase64Encoding(stringToEncode, PolyVideoOSRestAPI.Global.DefaultEncodingName);
        }

        /// <summary>
        /// Convert the given string to Base 64 Encoding based on the given encoding name
        /// </summary>
        /// <param name="stringToEncode">The string to convert to Base64</param>
        /// <param name="encodingName">The name of the encoding to use</param>
        /// <returns></returns>
        public static string ConvertToBase64Encoding(string stringToEncode, string encodingName)
        {
            string base64EncodedString = "";

            try
            {
                base64EncodedString = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding(encodingName).GetBytes(stringToEncode));
            }
            catch (Exception ex)
            {
                Debug.PrintExceptionToConsole(eDebugLevel.Error, ex, "NetworkHelperFunctions.ConvertToBase64Encoding(): Error Encoding String using encoding " + encodingName );
            }

            return base64EncodedString;
        }

        /// <summary>
        /// Convert the given string to Base 64 Encoding based on the given encoding codepage
        /// </summary>
        /// <param name="stringToEncode">The string to convert to Base64</param>
        /// <param name="codepage">The ID of the encoding codepage to use</param>
        /// <returns></returns>
        public static string ConvertToBase64Encoding(string stringToEncode, int codepage)
        {
            string base64EncodedString = "";

            try
            {
                base64EncodedString = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding(codepage).GetBytes(stringToEncode));
            }
            catch (Exception ex)
            {
                Debug.PrintExceptionToConsole(eDebugLevel.Error, ex, "NetworkHelperFunctions.ConvertToBase64Encoding(): Error Encoding String using encoding " + codepage);
            }

            return base64EncodedString;
        }

        /// <summary>
        /// Create the Base64 encoded basic authentication
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>The Base64 encoding of the username and password</returns>
        public static string CreateBasicAuthentiationBase64Encoding(string username, string password)
        {
            string base64AuthenticationString = "";

            if (!string.IsNullOrEmpty(username))
            {
                base64AuthenticationString = ConvertToDefaultBase64Encoding( String.Format( "{0}:{1}", username, password ) );
            }

            return base64AuthenticationString;
        }

        /// <summary>
        /// Returns just the Path portion of a URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetURLPath(string url)
        {
            return new UrlParser(url).Path;
        }

        /// <summary>
        /// Return the path and parameters of the URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetURLPathAndParameters(string url)
        {
            return new UrlParser(url).PathAndParams;
        }

        /// <summary>
        /// Returns the hostname portion of a URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetURLHostname(string url)
        {
            return new UrlParser(url).Hostname;
        }

        /// <summary>
        /// Returns the hostname and port of a URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetURLHostnameAndPort(string url)
        {
            return new UrlParser(url).HostnameAndPort;
        }

        /// <summary>
        /// Return the protocol portion of the URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetURLProtocol(string url)
        {
            return new UrlParser(url).Protocol;
        }
    }
}