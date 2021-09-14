namespace CCL.Simpl;
        // class declarations
         class SimplHelperFunctions;
    static class SimplHelperFunctions 
    {
        // class delegates

        // class events

        // class functions
        static INTEGER_FUNCTION UShortStringIsNullOrEmptry ( STRING s );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

namespace CCL;
        // class declarations
         class Global;
         class Extensions;
         class Lazy;
         class CommonUtilities;
         class ABaseDevice;
     class Global 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        static SIGNED_LONG_INTEGER DEFAULT_ENCODING_CODEPAGE;
        static STRING DEFAULT_ENCODING_NAME[];

        // class properties
    };

    static class Extensions 
    {
        // class delegates

        // class events

        // class functions
        static STRING_FUNCTION EmptyIfNull ( STRING input );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class Lazy 
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

    static class CommonUtilities 
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

namespace CCL.Feedback;
        // class declarations
         class FeedbackEventArgs;
         class AFeedbackBase;
         class eFeedbackType;
     class AFeedbackBase 
    {
        // class delegates

        // class events
        EventHandler FeedbackChanged ( AFeedbackBase sender, FeedbackEventArgs e );

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Key[];
        SIGNED_LONG_INTEGER IntegerValue;
        STRING StringValue[];
    };

    static class eFeedbackType // enum
    {
        static SIGNED_LONG_INTEGER None;
        static SIGNED_LONG_INTEGER Boolean;
        static SIGNED_LONG_INTEGER Integer;
        static SIGNED_LONG_INTEGER String;
    };

namespace CCL.Timer;
        // class declarations
         class PollingTimer;

namespace CCL.Network.REST;
        // class declarations
         class CCLGenericRestClientBase;
         class CCLHttpsRESTClient;
     class CCLGenericRestClientBase 
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

     class CCLHttpsRESTClient 
    {
        // class delegates

        // class events

        // class functions
        FUNCTION PrintDebugState ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        SIGNED_LONG_INTEGER PoolSize;
    };

namespace CCL.Network;
        // class declarations
         class NetworkDataMessageReceivedArgs;
         class RequestAuthType;
         class CCLHttpStatusCode;
         class CCLWebRequest;
         class CCLWebResponse;
         class NetworkHelperFunctions;
     class NetworkDataMessageReceivedArgs 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING IPOrHostname[];
        INTEGER Port;
        STRING Message[];
    };

    static class RequestAuthType // enum
    {
        static SIGNED_LONG_INTEGER NONE;
        static SIGNED_LONG_INTEGER BASIC;
    };

    static class CCLHttpStatusCode // enum
    {
        static SIGNED_LONG_INTEGER Continue;
        static SIGNED_LONG_INTEGER SwitchingProtocols;
        static SIGNED_LONG_INTEGER OK;
        static SIGNED_LONG_INTEGER Created;
        static SIGNED_LONG_INTEGER Accepted;
        static SIGNED_LONG_INTEGER NonAuthoritativeInformation;
        static SIGNED_LONG_INTEGER NoContent;
        static SIGNED_LONG_INTEGER ResetContent;
        static SIGNED_LONG_INTEGER PartialContent;
        static SIGNED_LONG_INTEGER MultipleChoices;
        static SIGNED_LONG_INTEGER MovedPermanently;
        static SIGNED_LONG_INTEGER Redirect;
        static SIGNED_LONG_INTEGER SeeOther;
        static SIGNED_LONG_INTEGER NotModified;
        static SIGNED_LONG_INTEGER UseProxy;
        static SIGNED_LONG_INTEGER Unused;
        static SIGNED_LONG_INTEGER TemporaryRedirect;
        static SIGNED_LONG_INTEGER BadRequest;
        static SIGNED_LONG_INTEGER Unauthorized;
        static SIGNED_LONG_INTEGER PaymentRequired;
        static SIGNED_LONG_INTEGER Forbidden;
        static SIGNED_LONG_INTEGER NotFound;
        static SIGNED_LONG_INTEGER MethodNotAllowed;
        static SIGNED_LONG_INTEGER NotAcceptable;
        static SIGNED_LONG_INTEGER ProxyAuthenticationRequired;
        static SIGNED_LONG_INTEGER RequestTimeout;
        static SIGNED_LONG_INTEGER Conflict;
        static SIGNED_LONG_INTEGER Gone;
        static SIGNED_LONG_INTEGER LengthRequired;
        static SIGNED_LONG_INTEGER PreconditionFailed;
        static SIGNED_LONG_INTEGER RequestEntityTooLarge;
        static SIGNED_LONG_INTEGER RequestUriTooLong;
        static SIGNED_LONG_INTEGER UnsupportedMediaType;
        static SIGNED_LONG_INTEGER RequestedRangeNotSatisfiable;
        static SIGNED_LONG_INTEGER ExpectationFailed;
        static SIGNED_LONG_INTEGER UpgradeRequired;
        static SIGNED_LONG_INTEGER InternalServerError;
        static SIGNED_LONG_INTEGER NotImplemented;
        static SIGNED_LONG_INTEGER BadGateway;
        static SIGNED_LONG_INTEGER ServiceUnavailable;
        static SIGNED_LONG_INTEGER GatewayTimeout;
        static SIGNED_LONG_INTEGER HttpVersionNotSupported;
    };

    static class NetworkHelperFunctions 
    {
        // class delegates

        // class events

        // class functions
        static STRING_FUNCTION ConvertToHex ( STRING messageString );
        static STRING_FUNCTION ConvertToDefaultBase64Encoding ( STRING stringToEncode );
        static STRING_FUNCTION ConvertToBase64Encoding ( STRING stringToEncode , STRING encodingName );
        static STRING_FUNCTION CreateBasicAuthentiationBase64Encoding ( STRING username , STRING password );
        static STRING_FUNCTION GetURLPath ( STRING url );
        static STRING_FUNCTION GetURLPathAndParameters ( STRING url );
        static STRING_FUNCTION GetURLHostname ( STRING url );
        static STRING_FUNCTION GetURLHostnameAndPort ( STRING url );
        static STRING_FUNCTION GetURLProtocol ( STRING url );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

namespace CCL.Hardware;
        // class declarations
         class ComPortBaudRate;
         class ComPortParity;
         class ComPortDataBits;
         class ComPortStopBits;
         class ComPortSoftwareHandshake;
         class ComPortHardwareHandshake;
         class ComPortProtocol;
         class ComPortReportCTS;
         class BasicProcessor;
    static class ComPortBaudRate // enum
    {
        static SIGNED_LONG_INTEGER Baud300;
        static SIGNED_LONG_INTEGER Baud600;
        static SIGNED_LONG_INTEGER Baud1200;
        static SIGNED_LONG_INTEGER Baud1800;
        static SIGNED_LONG_INTEGER Baud2400;
        static SIGNED_LONG_INTEGER Baud3600;
        static SIGNED_LONG_INTEGER Baud4800;
        static SIGNED_LONG_INTEGER Baud7200;
        static SIGNED_LONG_INTEGER Baud9600;
        static SIGNED_LONG_INTEGER Baud14400;
        static SIGNED_LONG_INTEGER Baud19200;
        static SIGNED_LONG_INTEGER Baud28800;
        static SIGNED_LONG_INTEGER Baud38400;
        static SIGNED_LONG_INTEGER Baud57600;
        static SIGNED_LONG_INTEGER Baud115200;
        static SIGNED_LONG_INTEGER GRAFIK_EYE;
        static SIGNED_LONG_INTEGER HOMEWORKS;
        static SIGNED_LONG_INTEGER SMPTE;
        static SIGNED_LONG_INTEGER MIDI;
    };

    static class ComPortParity // enum
    {
        static SIGNED_LONG_INTEGER ParityEven;
        static SIGNED_LONG_INTEGER ParityNone;
        static SIGNED_LONG_INTEGER ParityOdd;
        static SIGNED_LONG_INTEGER ParityZero;
    };

    static class ComPortDataBits // enum
    {
        static SIGNED_LONG_INTEGER DataBits7;
        static SIGNED_LONG_INTEGER DataBits8;
    };

    static class ComPortStopBits // enum
    {
        static SIGNED_LONG_INTEGER StopBits1;
        static SIGNED_LONG_INTEGER StopBits2;
    };

    static class ComPortSoftwareHandshake // enum
    {
        static SIGNED_LONG_INTEGER SH_NONE;
        static SIGNED_LONG_INTEGER SH_XONXOFF;
        static SIGNED_LONG_INTEGER SH_XONT;
        static SIGNED_LONG_INTEGER SH_XONR;
    };

    static class ComPortHardwareHandshake // enum
    {
        static SIGNED_LONG_INTEGER HW_NONE;
        static SIGNED_LONG_INTEGER HW_RTS;
        static SIGNED_LONG_INTEGER HW_CTS;
        static SIGNED_LONG_INTEGER HW_RTSCTS;
    };

    static class ComPortProtocol // enum
    {
        static SIGNED_LONG_INTEGER ProtocolRS232;
        static SIGNED_LONG_INTEGER ProtocolRS422;
        static SIGNED_LONG_INTEGER ProtocolRS485;
    };

    static class ComPortReportCTS // enum
    {
        static SIGNED_LONG_INTEGER OFF;
        static SIGNED_LONG_INTEGER ON;
    };

     class BasicProcessor 
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

namespace CCL.Switching;
        // class declarations
         class CommonSwitchingDevice;
         class CoreSwitchingController;
         class StringMap;
     class CommonSwitchingDevice 
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

     class CoreSwitchingController 
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

namespace CCL.Logging;
        // class declarations
         class CCLDebug;
         class eDebugLevel;
    static class CCLDebug 
    {
        // class delegates

        // class events

        // class functions
        static FUNCTION SetDebugLevel ( eDebugLevel newLevel );
        static FUNCTION PrintToConsole ( eDebugLevel level , STRING msg );
        static FUNCTION PrintToLog ( eDebugLevel level , STRING msg );
        static FUNCTION PrintToConsoleAndLog ( eDebugLevel level , STRING msg );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        eDebugLevel DebugLevel;
    };

    static class eDebugLevel // enum
    {
        static INTEGER Trace;
        static INTEGER Notice;
        static INTEGER Warning;
        static INTEGER Error;
    };

namespace CCL.Communications;
        // class declarations
         class MessageDataStringArgs;
         class MessageDataBytesArgs;
     class MessageDataStringArgs 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING MessageString[];
    };

     class MessageDataBytesArgs 
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

namespace CCL.Network.UDP;
        // class declarations
         class BasicUDPServer;
     class BasicUDPServer 
    {
        // class delegates

        // class events
        EventHandler DataReceived ( BasicUDPServer sender, NetworkDataMessageReceivedArgs e );

        // class functions
        FUNCTION Initialize ( STRING ipAddressOrHostname , INTEGER port );
        FUNCTION Connect ();
        FUNCTION Disconnect ();
        FUNCTION SendMessageString ( STRING message );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        INTEGER Port;
        STRING IPOrHostname[];
    };

namespace CCL.JSON;
        // class declarations
         class JSONStringMapConverter;
     class JSONStringMapConverter 
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

namespace CCL.Encryption;
        // class declarations
         class EncryptionHelper;
    static class EncryptionHelper 
    {
        // class delegates

        // class events

        // class functions
        static STRING_FUNCTION GetMD5Hash ( STRING source );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

namespace CCL.Queue;
        // class declarations

