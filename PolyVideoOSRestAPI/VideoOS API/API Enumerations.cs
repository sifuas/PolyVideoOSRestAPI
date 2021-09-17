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
    // type of payload syntax available to the API
    public enum eCommandSyntaxType
    {
        JSON = 1
    }

    /// <summary>
    /// how should the command input parameters be applied?
    /// PATH - add the parameter value to the end of the command path request
    ///     /rest/calendar/meetings/<meetingid>
    /// QUERY - add the parameter name and value as query parameters to the path or the request
    ///     /rest/calendar/meetings/?number=<integer>
    /// BODY_JSON - add the paremeter name and value as JSON to the body of the request
    ///     /rest/audio/acousticfence
    ///     Body:
    ///     {
    ///     "action": <string>,
    ///     "dtmfChar": <string>,
    ///     "speaker": <string>
    ///     }
    /// BODY_VALUE - add the parameter value as the body of the request
    ///     Body:
    ///     {
    ///     <boolean> | <integer> | <string>
    ///     }  
    /// </summary>
    public enum eCommandInputFormatType
    {        
        NONE        = 0,    // no parameter inputs are needed

        PATH        = 1,    // paremeter inputs added to the URL path of the request
        QUERY       = 2,    // parameter inputs added as a query to the URL of the request
        BODY_JSON   = 3,    // parameter added as JSON to the body of the request
        BODY_TEXT   = 4     // parameter added as plain text to the body of the request
    }

    /// <summary>
    /// The state of the session
    /// </summary>
    public enum eSessionState
    {
        LOGGED_OUT          = 0,        
        LOGGED_IN           = 1,

        LOGGING_IN          = 3,        

        INVALID_CREDENTIALS = 10,
        
        LOCKED_OUT          = 11,

        LOGIN_ERROR         = 20
    }

}