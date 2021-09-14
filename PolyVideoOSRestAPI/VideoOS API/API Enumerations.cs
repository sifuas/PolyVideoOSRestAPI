using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace MEI.Integration.PolyVideoOSRestAPI
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