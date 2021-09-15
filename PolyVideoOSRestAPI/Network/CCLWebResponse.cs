using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// crestron libraries
using Crestron.SimplSharp;
using RequestType = Crestron.SimplSharp.Net.Https.RequestType;

using MEI.Integration.PolyVideoOSRestAPI.Logging;

namespace MEI.Integration.PolyVideoOSRestAPI.Network
{

    /// <summary>
    /// Encapsulate the response from a http or https web request
    /// </summary>
    public class CCLWebResponse : ICCLDebuggable
    {
        // status code of the request        
        public CCLHttpStatusCode StatusCode { get; private set; }

        // value of any content        
        public string Content { get; private set; }

        // URL stored in the response        
        public string ResponseURL { get; private set; }

        // store the original request type
        public RequestType OriginalRequestType { get; private set; }

        // store the response headers
        public Dictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Create a response object to hold the data returned from a web request.
        /// </summary>
        /// <param name="status">The HTTP status code</param>
        /// <param name="content">Content returned from the request</param>
        /// <param name="responseURL">URL returned from the request</param>
        public CCLWebResponse(CCLHttpStatusCode status, string content, string responseURL, Dictionary<string,string> responseHeaders, RequestType requestType )
        {
            StatusCode = status;
            Content = content;
            ResponseURL = responseURL;
            Headers = responseHeaders;
            OriginalRequestType = requestType;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("WebResponseResult : Status - ");
            str.Append(StatusCode);

            str.Append(" \nURL - ");
            if (ResponseURL != null)
                str.Append(ResponseURL);

            if (Headers != null)
            {
                str.Append(" \nHeaders :");

                foreach (KeyValuePair<string, string> header in Headers)
                {
                    str.Append(String.Format(" \n{0} = ", header.Key ));
                    if (header.Value != null)
                        str.Append(header.Value);
                    else
                        str.Append("NULL");
                }
            }

            str.Append(" \nContent - ");
            if (Content != null)
                str.Append(Content);

            return str.ToString();
        }

        #region ICCLDebuggable Members

        public void PrintDebugState()
        {
            CrestronConsole.PrintLine(this.ToString());
        }

        #endregion
    }
}