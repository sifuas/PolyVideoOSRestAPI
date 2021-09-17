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

// crestron libraries
using Crestron.SimplSharp;
using RequestType = Crestron.SimplSharp.Net.Https.RequestType;

using PolyVideoOSRestAPI.Logging;

namespace PolyVideoOSRestAPI.Network
{

    /// <summary>
    /// Encapsulate the response from a http or https web request
    /// </summary>
    public class WebResponse : IDebuggable
    {
        // status code of the request        
        public HttpStatusCode StatusCode { get; private set; }

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
        public WebResponse(HttpStatusCode status, string content, string responseURL, Dictionary<string,string> responseHeaders, RequestType requestType )
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

        #region IDebuggable Members

        public void PrintDebugState()
        {
            CrestronConsole.PrintLine(this.ToString());
        }

        #endregion
    }
}