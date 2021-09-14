namespace MEI.Integration.PolyVideoOSRestAPI;
        // class declarations
         class PolyVideoOSDevice;
         class APIEventArgsBase;
         class APIUnknownResponseEventArgs;
         class APIErrorResponseEventArgs;
         class APISessionStateEventArgs;
         class APISystemModeStateEventArgs;
         class APIGenericStateEventArgs;
         class eCommandSyntaxType;
         class eCommandInputFormatType;
         class eSessionState;
           class SessionStateChangedEventArgs;
           class UShortChangeEventArgs;
           class ErrorChangeEventArgs;
           class APISessionStateObject;
           class APISystemModeObject;
     class PolyVideoOSDevice 
    {
        // class delegates

        // class events
        EventHandler OnSessionStateChanged ( PolyVideoOSDevice sender, SessionStateChangedEventArgs e );
        EventHandler OnDevideModeChanged ( PolyVideoOSDevice sender, UShortChangeEventArgs e );
        EventHandler OnErrorStateChanged ( PolyVideoOSDevice sender, ErrorChangeEventArgs e );

        // class functions
        INTEGER_FUNCTION Connect ();
        INTEGER_FUNCTION Disconnect ();
        INTEGER_FUNCTION Reboot ();
        INTEGER_FUNCTION StartDeviceMode ();
        INTEGER_FUNCTION StopDeviceMode ();
        FUNCTION PrintDebugState ();
        FUNCTION Dispose ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING HostNameOrIPAddress[];
        STRING Username[];
        STRING Password[];
        INTEGER DebugState;
        INTEGER Connected;
    };

     class APIEventArgsBase 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class APISessionStateEventArgs 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        eSessionState State;
        STRING SessionID[];
        STRING StringState[];
        APISessionStateObject SessionState;
        INTEGER UShortState;
    };

     class APISystemModeStateEventArgs 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        APISystemModeObject SystemMode;
    };

     class APIGenericStateEventArgs 
    {
        // class delegates

        // class events

        // class functions
        INTEGER_FUNCTION GetResponseCode ();
        STRING_FUNCTION GetResponseContent ();
        STRING_FUNCTION GetResponseURL ();
        SIGNED_LONG_INTEGER_FUNCTION GetHeaderCount ();
        STRING_FUNCTION GetHeaderKey ( SIGNED_LONG_INTEGER index );
        STRING_FUNCTION GetHeaderValue ( SIGNED_LONG_INTEGER index );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

    static class eCommandSyntaxType // enum
    {
        static SIGNED_LONG_INTEGER JSON;
    };

    static class eCommandInputFormatType // enum
    {
        static SIGNED_LONG_INTEGER NONE;
        static SIGNED_LONG_INTEGER PATH;
        static SIGNED_LONG_INTEGER QUERY;
        static SIGNED_LONG_INTEGER BODY_JSON;
        static SIGNED_LONG_INTEGER BODY_TEXT;
    };

    static class eSessionState // enum
    {
        static SIGNED_LONG_INTEGER LOGGED_OUT;
        static SIGNED_LONG_INTEGER LOGGED_IN;
        static SIGNED_LONG_INTEGER LOGGING_IN;
        static SIGNED_LONG_INTEGER INVALID_CREDENTIALS;
        static SIGNED_LONG_INTEGER LOCKED_OUT;
        static SIGNED_LONG_INTEGER LOGIN_ERROR;
    };

namespace MEI.Integration.PolyVideoOSRestAPI.API_Objects;
        // class declarations
         class APIObjectBase;
         class APISessionStateObject;
         class InternalSessionStatus;
         class InternalSessionState;
         class APISystemModeObject;
     class APIObjectBase 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING OriginalContent[];
    };

     class APISessionStateObject 
    {
        // class delegates

        // class events

        // class functions
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        InternalSessionStatus LoginStatus;
        InternalSessionState Session;
        STRING Reason[];
        INTEGER UShortSuccess;
        STRING OriginalContent[];
    };

     class InternalSessionStatus 
    {
        // class delegates

        // class events

        // class functions
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        INTEGER FailedLogins;
        STRING LastLoginClient[];
        STRING LastLoginClientType[];
        STRING LoginResult[];
    };

     class InternalSessionState 
    {
        // class delegates

        // class events

        // class functions
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING ClientType[];
        STRING Location[];
        STRING Role[];
        STRING SessionID[];
        STRING UserID[];
    };

     class APISystemModeObject 
    {
        // class delegates

        // class events

        // class functions
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING ActivePersona[];
        STRING App[];
        STRING AppPersona[];
        STRING EcoMode[];
        STRING Mode[];
        STRING OriginalContent[];
    };

namespace MEI.Integration.PolyVideoOSRestAPI.Simpl_Interface;
        // class declarations
         class SessionStateChangedEventArgs;
     class SessionStateChangedEventArgs 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING StringState[];
        eSessionState State;
        INTEGER UState;
        INTEGER UConnected;
    };

namespace MEI.Integration.PolyVideoOSRestAPI.Events;
        // class declarations
         class BooleanChangeEventArgs;
         class UShortChangeEventArgs;
         class StringChangeEventArgs;
         class ErrorChangeEventArgs;
     class BooleanChangeEventArgs 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        INTEGER UShortValue;
        INTEGER EventType;
    };

     class UShortChangeEventArgs 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        INTEGER EventType;
        INTEGER Value;
        INTEGER LastValue;
    };

     class StringChangeEventArgs 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        INTEGER EventType;
        STRING Value[];
        STRING LastValue[];
    };

     class ErrorChangeEventArgs 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        INTEGER UShortIsError;
        INTEGER EventType;
        STRING Value[];
        STRING LastValue[];
    };

namespace MEI.Integration.PolyVideoOSRestAPI.Queue;
        // class declarations

namespace MEI.Integration.PolyVideoOSRestAPI.InputCommands;
        // class declarations

