using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// crestron libraries
using Crestron.SimplSharp;
using RequestType = Crestron.SimplSharp.Net.Https.RequestType;

namespace MEI.Integration.PolyVideoOSRestAPI.Network.REST
{
    public abstract class CCLGenericRestClientBase
    {
        public abstract CCLWebResponse SubmitRequest(CCLWebRequest request);

        protected string GenerateURL(CCLWebRequest request, bool secure )
        {
            string protocol = "http://";
            if (secure)
                protocol = "https://";

            // create the URL to connect to
            StringBuilder fullURLString = new StringBuilder();

            // add the protocol
            if (!request.Host.ToLower().StartsWith(protocol))
                fullURLString.Append(protocol);

            // append the host information
            fullURLString.Append(request.Host);

            // add the port if it is valid
            if (request.Port > 0 && request.Port <= 65535)
            {
                fullURLString.Append(":");
                fullURLString.Append(request.Port);
            }

            // add any path information to the URL
            if ((request.Path != null) && (request.Path.Length > 0))
            {
                if (!request.Host.EndsWith("/") && !request.Path.StartsWith("/"))
                    fullURLString.Append("/");

                fullURLString.Append(request.Path);
            }

            return fullURLString.ToString();
        }
    }
}