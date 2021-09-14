using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace MEI.Integration.PolyVideoOSRestAPI
{
    /// <summary>
    /// Define global constants and functions
    /// </summary>
    internal static class APIGlobal
    {

        // the base path for the rest API
        public static readonly string API_BASE_PATH = "/rest";

        // USB device mode
        public static readonly ushort DEVICE_MODE_START = 1;
        public static readonly ushort DEVICE_MODE_STOP  = 2;

        // rest path labels
        public static readonly string API_SESSION                   = "session";            // login and logout of the session
        public static readonly string API_DEVICE_MODE               = "system/mode/device"; // set the device mode
        public static readonly string API_REBOOT                    = "system/reboot";      // reboot the device
        public static readonly string API_SYSTEM_MODE_STATUS        = "system/mode";        // Get the device mode status

        /// <summary>
        /// Combine the API base path with the subpath provided to create the full path for the REST request
        /// </summary>
        /// <param name="apiSubPath"></param>
        /// <returns></returns>
        public static string CreateFullRESTAPIPath(string apiSubPath)
        {
            StringBuilder fullURLPath = new StringBuilder();

            if (!apiSubPath.StartsWith(APIGlobal.API_BASE_PATH))
                fullURLPath.Append(APIGlobal.API_BASE_PATH);

            if (!apiSubPath.StartsWith("/"))
                fullURLPath.Append("/");

            fullURLPath.Append(apiSubPath);

            return fullURLPath.ToString();
        }

    }
}