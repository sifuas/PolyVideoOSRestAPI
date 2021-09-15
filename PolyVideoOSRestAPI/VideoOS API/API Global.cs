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