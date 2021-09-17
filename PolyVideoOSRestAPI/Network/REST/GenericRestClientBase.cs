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

namespace PolyVideoOSRestAPI.Network.REST
{
    public abstract class GenericRestClientBase
    {
        public abstract WebResponse SubmitRequest(WebRequest request);

        protected string GenerateURL(WebRequest request, bool secure )
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