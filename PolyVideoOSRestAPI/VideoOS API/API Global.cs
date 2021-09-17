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

namespace PolyVideoOSRestAPI
{
    /// <summary>
    /// Define global constants and functions
    /// </summary>
    internal static class APIGlobal
    {
        // the base path for the rest API
        public static readonly string ApiBasePath = "/rest";

        // USB device mode
        public static readonly ushort DeviceModeStart = 1;
        public static readonly ushort DeviceModeStop  = 2;

        // rest path labels
        public static readonly string ApiSession                   = "session";            // login and logout of the session
        public static readonly string ApiDeviceMode               = "system/mode/device"; // set the device mode
        public static readonly string ApiReboot                    = "system/reboot";      // reboot the device
        public static readonly string ApiSystemModeStatus        = "system/mode";        // Get the device mode status

        /// <summary>
        /// Combine the API base path with the subpath provided to create the full path for the REST request
        /// </summary>
        /// <param name="apiSubPath"></param>
        /// <returns></returns>
        public static string CreateFullRESTAPIPath(string apiSubPath)
        {
            StringBuilder fullURLPath = new StringBuilder();

            if (!apiSubPath.StartsWith(APIGlobal.ApiBasePath))
                fullURLPath.Append(APIGlobal.ApiBasePath);

            if (!apiSubPath.StartsWith("/"))
                fullURLPath.Append("/");

            fullURLPath.Append(apiSubPath);

            return fullURLPath.ToString();
        }

    }
}