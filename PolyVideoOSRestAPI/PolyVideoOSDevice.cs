// system libraries
using System;
using System.Text;
using System.Collections.Generic;

// crestron imports
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net.Https;

using MEI.Integration.PolyVideoOSRestAPI.Simpl_Interface;
using MEI.Integration.PolyVideoOSRestAPI.Logging;
using MEI.Integration.PolyVideoOSRestAPI.Network;
using MEI.Integration.PolyVideoOSRestAPI.Network.REST;
using MEI.Integration.PolyVideoOSRestAPI.Timer;
using HttpStatusCode = MEI.Integration.PolyVideoOSRestAPI.Network.CCLHttpStatusCode;

using MEI.Integration.PolyVideoOSRestAPI.Events;
using MEI.Integration.PolyVideoOSRestAPI.ResponseHandlers;
using MEI.Integration.PolyVideoOSRestAPI.InputCommands;
using MEI.Integration.PolyVideoOSRestAPI.Queue;

namespace MEI.Integration.PolyVideoOSRestAPI
{
    /// <summary>
    /// Class to connect to the VideoOS device and process responses
    /// </summary>
    public sealed class PolyVideoOSDevice : IDisposable, ICCLDebuggable
    {

        // Simpl + delegates
        public event EventHandler<SessionStateChangedEventArgs> OnSessionStateChanged;
        public event EventHandler<UShortChangeEventArgs> OnDevideModeChanged;
        public event EventHandler<ErrorChangeEventArgs> OnErrorStateChanged;

        //public event UShortStateChangedDelegate OnGenericUshortChanged;
        //public event StringStateChangedDelegate OnGenericStringChanged;

        ///
        /// Constant values
        ///
        private const int SESSION_DEFAULT_TIME      = 1000;
        private const int SESSION_EXTEND_TIME       = 10000;

        ///
        /// Private class members
        ///

        // has the class been disposed?
        private bool _disposed = false;

        // store the session ID for communicating to the codec
        private APISession apiSession;
               
        // store a list of handlers to use for parsing responses
        private Dictionary<string, APIResponseHandlerBase> responseHandlersCollection;

        // store a list of commands to send to the device
        private Dictionary<string, APICommand> apiCommandCollection; 

        // create a queue to hold the commands to send. This will be dequed by a separate thread
        private CommandProcessingQueue sendCommandProcessingQueue;

        // create a queue to process the response handlers. This will be dequed by a separate thread
        private ResponseHandlerProcessingQueue responseHandlerProcessingQueue;        

        // timers to handle polling
        private CTimer loginTimer;
        private CTimer statusPollingTimer;
        private CTimer logoutResponseTimer;

        #region Class Properties
        ///
        /// Properties
        ///

        // connection string
        public string HostNameOrIPAddress
        {
            get
            {
                return ((apiSession != null) ? apiSession.HostnameOrIPAddress : "");
            }
            set
            {
                if (apiSession != null)
                    apiSession.HostnameOrIPAddress = value;
            }
        }

        public string Username
        {
            get { return apiSession.Credentials.Username; }
            set
            {
                apiSession.Credentials.Username = value;
            }
        }

        public string Password
        {
            get { return apiSession.Credentials.Password; }
            set { apiSession.Credentials.Password = value; }
        }

        private bool debugState;
        public ushort DebugState
        {
            get { return (ushort)(debugState ? 1 : 0); }
            set
            {
                debugState = ( value == 0 ) ? false : true;

                if (debugState)
                {
                    CrestronConsole.PrintLine("{0} - Debug Enabled", this.GetType().Name);
                    CCLDebug.SetDebugLevel(eDebugLevel.Trace);
                    PrintDebugState();
                }
                else
                {
                    CrestronConsole.PrintLine("{0} - Debug Disabled", this.GetType().Name);
                    CCLDebug.SetDebugLevel(eDebugLevel.Error);
                }
            }
        }

        /// <summary>
        /// The current connection state
        /// </summary>
        public ushort Connected
        {
            get { return (ushort)(apiSession.Connected ? 1 : 0); }
        }

        /// <summary>
        /// The time in milliseconds to use for polling
        /// </summary>
        private ushort _pollingTime = 2000;
        public ushort PollingTime 
        {
            get
            {
                return _pollingTime;
            }
            set
            {
                if( value > 0 )
                    _pollingTime = value;
            }
        }

        #endregion



        /// <summary>
        /// SIMPL+ can only execute the default constructor. If you have variables that require initialization, please
        /// use an Initialize method
        /// </summary>
        public PolyVideoOSDevice()
        {       
            apiSession = new APISession();

            // store commands and response handlers
            responseHandlersCollection      = new Dictionary<string, APIResponseHandlerBase>();
            apiCommandCollection            = new Dictionary<string, APICommand>();

            // create the processing threads to handle sending commands and processing responses
            responseHandlerProcessingQueue  = new ResponseHandlerProcessingQueue("Response Queue", responseHandlersCollection);            

            sendCommandProcessingQueue      = new CommandProcessingQueue("Command Queue", apiSession );
            sendCommandProcessingQueue.OnQueueProcessingError           += HandleCommandQueueProcessingException;
            sendCommandProcessingQueue.OnResponseReceived               += HandleCommandQueueResponseReceived;

            // add the standard headers needed
            apiSession.HTTPHeaders.Add("Content-Type", "application/json");

            // initialize timers
            loginTimer                      = new CTimer(LoginTimerCallback, null, Timeout.Infinite, Timeout.Infinite); 
            statusPollingTimer              = new CTimer(StatusPollingTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
            logoutResponseTimer             = new CTimer(LogoutResponseTimerCallback, null, Timeout.Infinite, Timeout.Infinite);

            // setup commands to send and handlers to take care of the received responses
            InitializeAPICommands();
            InitializeAPIResponseHandlers();

            // register events with the Crestron processor to properly handle shutdown and network changes
            CrestronEnvironment.ProgramStatusEventHandler   += new ProgramStatusEventHandler( PolyVideoOSDevice_ProgramStatusEventHandler );
            CrestronEnvironment.EthernetEventHandler        += new EthernetEventHandler( PolyVideoOSDevice_EthernetEventHandler );
        }


        #region Command Functions to the VideoOS Device
        /// <summary>
        /// Open a session and connect to the device
        /// </summary>
        /// <returns></returns>
        public ushort Connect()
        {
            CCLDebug.PrintToConsole(eDebugLevel.Trace, "{0}.Connect() : Started", this.GetType().Name);

            if (String.IsNullOrEmpty(HostNameOrIPAddress))
                throw new InvalidOperationException("No Host or IP Address Set");

            if (!apiSession.AuthenticationIsValid())
                throw new InvalidOperationException("Invalid Authentication data. Username or password are not set");

            try
            {
                loginTimer.Stop();
                statusPollingTimer.Stop();
                sendCommandProcessingQueue.Clear();                    

                // set flag indicating that the system should be connected
                apiSession.ConnectionRequested = true;

                SetSessionState(eSessionState.LOGGING_IN, apiSession.Connected);

                // send the login session command
                APICommand command = GetCommand(APIGlobal.API_SESSION);

                if (command != null)
                {
                    CCLDebug.PrintToConsole(eDebugLevel.Trace, "Adding Command to Queue : {0}", command);
                    sendCommandProcessingQueue.Enqueue(command);
                }
                else
                {
                    CCLDebug.PrintToConsole(eDebugLevel.Warning, "{0}.Connect() : NULL Command retrieved from Queue", this.GetType().Name);
                }

                loginTimer.Reset(1000, 10000); 
            }
            catch (Exception ex)
            {
                PrintDebugMessage("{0}.Connect( ) Error = {1}", this.GetType().Name, ex.Message);
                PrintDebugMessage("{0}.Connect( ) Error = {1}", this.GetType().Name, ex.StackTrace);
                return 0;
            }

            return 1;
        }

        /// <summary>
        /// Disconnect the connection to the VideoOS device
        /// </summary>
        public ushort Disconnect()
        {
            apiSession.ConnectionRequested = false;

            loginTimer.Stop();
            statusPollingTimer.Stop();

            sendCommandProcessingQueue.Clear();
            responseHandlerProcessingQueue.Clear();

            // only send the logout command if we are currently connected
            if (apiSession.Connected)
            {
                SendCommand(new APICommand(APIGlobal.API_SESSION, RequestType.Delete, false));
                logoutResponseTimer.Reset(5000, Timeout.Infinite);
            }
            else
            {
                SetSessionState(eSessionState.LOGGED_OUT, false);
            }

            return 1;
        }

        /// <summary>
        /// Reboot the system
        /// </summary>
        /// <returns></returns>
        public ushort Reboot()
        {
            if (!apiSession.Connected)
                return 0;

            SendCommand(new APICommand(APIGlobal.API_REBOOT, RequestType.Post, false));
            return 1;
        }

        /// <summary>
        /// Start Device Mode
        /// </summary>
        /// <returns></returns>
        public ushort StartDeviceMode()
        {
            if (!apiSession.Connected)
                return 0;

            SendCommand(new APICommand(APIGlobal.API_DEVICE_MODE, RequestType.Post, false));
            SendCommand(APIGlobal.API_SYSTEM_MODE_STATUS);
            return 1;
        }

        /// <summary>
        /// Stop the device mode
        /// </summary>
        /// <returns></returns>
        public ushort StopDeviceMode()
        {
            if (!apiSession.Connected)
                return 0;

            SendCommand(new APICommand(APIGlobal.API_DEVICE_MODE, RequestType.Delete, false));
            SendCommand(GetCommand(APIGlobal.API_SYSTEM_MODE_STATUS));

            return 1;
        }

        #endregion






        #region CTimer function callbacks
        /// <summary>
        /// Called when the login timer thread expires
        /// </summary>
        /// <param name="loginTimerCallbackObject"></param>
        private void LoginTimerCallback(Object loginTimerCallbackObject)
        {
            // if the session is not connected then we need to keep trying to log in
            if (apiSession.ConnectionRequested == true)
            {
                //PrintDebugMessage("***** {0}.LoginTimerCallback( ) : Queueing Login Command. Connected = {1}, Requested = {2}", this.GetType().Name, apiSession.Connected, apiSession.ConnectionRequested );

                // clear out any old commands in case the processing thread isn't working fast enough to actually deque the commands
                sendCommandProcessingQueue.Clear();

                // queue a new command
                sendCommandProcessingQueue.Enqueue(GetCommand(APIGlobal.API_SESSION));
                
            }
            else
            {
                //PrintDebugMessage("***** {0}.LoginTimerCallback( ) : Session Connected = {1}, Requested = {2}", this.GetType().Name, apiSession.Connected, apiSession.ConnectionRequested);
                loginTimer.Stop();
            }
        }

        /// <summary>
        /// Called when the status polling timer expires
        /// </summary>
        /// <param name="statusPollingTimerCallbackObject"></param>
        private void StatusPollingTimerCallback(object statusPollingTimerCallbackObject)
        {
            if (apiSession.Connected == true)
            {
                //PrintDebugMessage("***** {0}.StatusPollingTimerCallback( ) : Queueing Poll Commands. Connected = {1}", this.GetType().Name, apiSession.Connected);
                CrestronInvoke.BeginInvoke(thread => EnqueueStatusPollingCommands());
            }
            else
            {
                PrintDebugMessage("***** {0}.StatusPollingTimerCallback( ) : Stopping Timer", this.GetType().Name);
                statusPollingTimer.Stop();
            }
        }

        /// <summary>
        /// Callback function to handle when a disconnect API event doesn't return a response.
        /// </summary>
        /// <param name="disconnectResponseTimerCallbackObjet"></param>
        private void LogoutResponseTimerCallback(object disconnectResponseTimerCallbackObjet)
        {
            // remove any old session cookie
            if (apiSession.HTTPHeaders.ContainsKey("Cookie"))
            {
                apiSession.HTTPHeaders.Remove("Cookie");
            }

            loginTimer.Stop();
            statusPollingTimer.Stop();

            if (sendCommandProcessingQueue.Count > 0)
            {
                sendCommandProcessingQueue.Clear();
            }

            SetSessionState(eSessionState.LOGGED_OUT, false);
        }
        #endregion





        #region Response Handler - Callback functions for specific response handlers
        /// <summary>
        /// Handle the state change for the session
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void SessionStateChanged(object sender, APISessionStateEventArgs eventArgs)
        {

            // check if the session login was successful
            if ( eventArgs.State == eSessionState.LOGGED_IN )
            {

                //PrintDebugMessage("***** {0}.SessionStateChanged( ) : Logged In", this.GetType().Name);

                // the session ID is only returned upon a successful login. If we are already logged in the session ID is not returned
                if (eventArgs.State == eSessionState.LOGGED_IN)
                    apiSession.SessionID = eventArgs.SessionID;

                // remove any old session cookie
                if (apiSession.HTTPHeaders.ContainsKey("Cookie"))
                {
                    apiSession.HTTPHeaders.Remove("Cookie");
                }

                // add the session id back in as a cookie
                apiSession.HTTPHeaders.Add("Cookie", apiSession.GetSessionCookie());

                // stop the login timer since we are successfully logged in
                loginTimer.Stop();

                // clear any other commands before polling
                // TODO : Update Queue to a priority queue to prioritize commands over polling
                sendCommandProcessingQueue.Clear();

                apiSession.Connected = true ;

                //Initializing = true;
                apiSession.State = eSessionState.LOGGED_IN;

                // once we are logged in send polling commands and start polling timer
                EnqueueStatusPollingCommands();
                statusPollingTimer.Reset(PollingTime, PollingTime);

                ClearErrorFeedback();
                
            }
            // logged out
            else if (eventArgs.State == eSessionState.LOGGED_OUT)
            {
                logoutResponseTimer.Stop();

               // PrintDebugMessage("***** {0}.SessionStateChanged( ) : Logged Out", this.GetType().Name);

                // remove any old session cookie
                if (apiSession.HTTPHeaders.ContainsKey("Cookie"))
                {
                    apiSession.HTTPHeaders.Remove("Cookie");
                }                

                loginTimer.Stop();
                statusPollingTimer.Stop();

                apiSession.Connected = false;
                apiSession.State = eSessionState.LOGGED_OUT;

                if (sendCommandProcessingQueue.Count > 0 )
                {
                    sendCommandProcessingQueue.Clear();
                }                
            }
            // too many failed login attempts and are locked out
            else if (eventArgs.State == eSessionState.LOCKED_OUT)
            {
                CCLDebug.PrintToConsoleAndLog(eDebugLevel.Error, "{0}.SessionStateChanged( ) : Locked Out, too many login attempts to {1}. User = {2}", this.GetType().Name, apiSession.HostnameOrIPAddress, apiSession.Credentials.Username);

                // remove any old session cookie
                if (apiSession.HTTPHeaders.ContainsKey("Cookie"))
                {
                    apiSession.HTTPHeaders.Remove("Cookie");
                }
                
                if( apiSession.ConnectionRequested == true )
                    loginTimer.Reset(120000, 120000); // StartExtended();

                statusPollingTimer.Stop();

                apiSession.Connected = false;
                apiSession.State = eSessionState.LOCKED_OUT;

            }
            else if (eventArgs.State == eSessionState.INVALID_CREDENTIALS)
            {
                CCLDebug.PrintToConsoleAndLog(eDebugLevel.Error, "{0}.SessionStateChanged( ) : Invalid Credentials for {1}. User = {2}", this.GetType().Name, apiSession.HostnameOrIPAddress, apiSession.Credentials.Username);

                apiSession.Connected = false;
                apiSession.State = eSessionState.INVALID_CREDENTIALS;

            }
            else if (eventArgs.State == eSessionState.LOGIN_ERROR) // otherwise assume that we need to login again
            {
                CCLDebug.PrintToConsoleAndLog(eDebugLevel.Error, "{0}.SessionStateChanged( ) : Error Logging In to {1}", this.GetType().Name, apiSession.HostnameOrIPAddress);

                if (apiSession.ConnectionRequested == true)
                    loginTimer.Reset(120000, 120000); // StartExtended();

                apiSession.Connected = false;
                apiSession.State = eSessionState.LOGIN_ERROR;
            }

            PrintDebugMessage("{0}.SessionStateChanged( ) : Session after state change * ( {1} )", this.GetType().Name, eventArgs.State);

            UpdateSessionStateFeedback();
        }

        /// <summary>
        /// Handle response when the system mode changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void SystemModeStatusChanged(object sender, APISystemModeStateEventArgs eventArgs)
        {
            PrintDebugMessage("{0}.SystemModeStatusChanged( ) : Device Mode = {1}", this.GetType().Name, eventArgs.SystemMode);

            // send event when the device mode changes. this indicates whether USB device mode is on or off
            CrestronInvoke.BeginInvoke(thread => OnDevideModeChanged(this, new UShortChangeEventArgs(SimplHelperFunctions.ConvertBooleanToUShort(eventArgs.SystemMode.DeviceModeEnabled), 0)));

            //TODO : Add additional logic for feedback of whether the video OS device is setup for Poly, Zoom, Teams, etc...
        } 
        #endregion





        #region Response Handler - Callback functions for any responses that had no response handler defined or the handler could not handle the response that was returned for it
        /// <summary>
        /// If no specific response handler was defined for an API path but a message was received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ResponserHandlerUnregisteredResponseReceived(object sender, APIGenericStateEventArgs eventArgs)
        {
            if (eventArgs.Response != null)
            {
                string urlPath = NetworkHelperFunctions.GetURLPath(eventArgs.Response.ResponseURL);
                string genericPath = APIGlobal.CreateFullRESTAPIPath("*");

                CCLDebug.PrintToConsoleAndLog(eDebugLevel.Error, "{0}.ResponserHandlerUnregisteredResponseReceived() :  URL Path = {2}, Generic Path = {3}", this.GetType().Name, urlPath, genericPath);

                if (CCLDebug.CheckDebugLevel(eDebugLevel.Trace))
                {
                    foreach (string key in responseHandlersCollection.Keys)
                    {
                        CrestronConsole.PrintLine(" Key = {0}", key);
                    }
                }

                if ( (eventArgs.Response.StatusCode >= HttpStatusCode.InternalServerError)  && ( eventArgs.Response.StatusCode <= HttpStatusCode.HttpVersionNotSupported))
                {
                    UpdateErrorFeedback(true, String.Format("{0} - {1}", eventArgs.Response.StatusCode, eventArgs.Response.StatusCode.ToString()));              
                }                
            }
        }

        /// <summary>
        /// Handle a server error response if the specific response handler was not able to.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ReponseHandlerServerErrorResponseReceived(object sender, APIErrorResponseEventArgs eventArgs)
        {
            if (eventArgs.Response != null)
            {
                // 500 errors probably indicate an issue with the VideoOS device and it probably needs to be rebooted
                // since we can't do that we can try to login again
                if ((apiSession.ConnectionRequested == true) && ((eventArgs.Response.StatusCode >= HttpStatusCode.InternalServerError) && (eventArgs.Response.StatusCode <= HttpStatusCode.HttpVersionNotSupported)))
                {
                    SessionReconnect( );
                }
            }
        }

        /// <summary>
        /// Handle a valid API response that was not handled by the specific response handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ResponseHandlerUnhandledResponseReceived(object sender, APIUnknownResponseEventArgs eventArgs)
        {
            CCLWebResponse response = eventArgs.Response;

            // check the content returned and the return code. If 403/Forbidden is returned and the content is HTML then we are probably not logged in
            if ((apiSession.ConnectionRequested == true)
                && ((NetworkHelperFunctions.HTTPStatusIsClientError(response.StatusCode)) || (NetworkHelperFunctions.HTTPStatusIsServerError(response.StatusCode))) 
                && response.Content.EmptyIfNull().Contains("<html>"))
            {
                SessionReconnect( SESSION_DEFAULT_TIME, SESSION_DEFAULT_TIME );
            }
        } 
        #endregion




        #region Queue Processing Callbacks

        /// <summary>
        /// Handle the response received by sending the command to the API
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void HandleCommandQueueResponseReceived(object sender, CCLWebResponse response)
        {
            if( response != null )
                responseHandlerProcessingQueue.Enqueue(response);
        }

        /// <summary>
        /// Handle exception when processing the command queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        private void HandleCommandQueueProcessingException(object sender, Exception ex)
        {
            UpdateErrorFeedback(true, ex.Message);

            // we should only receive an HTTPS exception which means we can't connect to the device and should try to login if we have already connected before
            if (( apiSession.ConnectionRequested == true ) && (ex is HttpsException) )
            {
                SessionReconnect( );                
            }
            else
            {
                CCLDebug.PrintExceptionToConsoelAndLog(eDebugLevel.Error, ex, "{0}.HandleCommandQueueProcessingException( )", this.GetType().Name);
                
            }
        }

        /// <summary>
        /// handle exception when processing the response queue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        private void HandleResponseQueueProcessingException(object sender, Exception ex)
        {
            UpdateErrorFeedback(true, ex.Message);
            PrintDebugMessage("{0}.HandleResponseQueueProcessingError( )", this.GetType().Name);
            PrintDebugMessage(ex);
        } 
        #endregion






        #region Initialize Commands and Response Handlers
        /// <summary>
        /// Setup the commands needed for the api
        /// </summary>
        private void InitializeAPICommands()
        {
            // login commands
            APICommand command = new APICommand(APIGlobal.API_SESSION, eCommandInputFormatType.BODY_JSON, ((APIInputCommand)apiSession), null, RequestType.Post, true, true, 10);
            apiCommandCollection.Add(command.Path, command);

            // device mode status : gets device mode, provider, etc.. ( is the device setup for Poly, Zoom, etc... )
            command = new APICommand(APIGlobal.API_SYSTEM_MODE_STATUS, eCommandInputFormatType.NONE, null, null, RequestType.Get, true, true, 10);
            apiCommandCollection.Add(command.Path, command);

            // add additional command objects
        }

        /// <summary>
        /// Setup the response handlers that deal with the responses from any commands
        /// </summary>
        private void InitializeAPIResponseHandlers()
        {
            // generic response handler that will be called whenever a response is received that doesn't have a specific handler assigned
            DefaultResponseHandler genericHandler = new DefaultResponseHandler(apiSession, "*");
            genericHandler.OnUnknownResponseReceived += ResponseHandlerUnhandledResponseReceived;
            genericHandler.OnErrorResponseReceived += ReponseHandlerServerErrorResponseReceived;
            genericHandler.OnDefaultResponseHandlerResponseReceived += ResponserHandlerUnregisteredResponseReceived;
            responseHandlersCollection.Add(genericHandler.GetFullAPIPath(), genericHandler);

            // session handler
            SessionResponseHandler sessionHandler = new SessionResponseHandler(apiSession, APIGlobal.API_SESSION);
            sessionHandler.OnUnknownResponseReceived += ResponseHandlerUnhandledResponseReceived;
            sessionHandler.OnErrorResponseReceived += ReponseHandlerServerErrorResponseReceived;
            sessionHandler.OnSessionStateChanged += SessionStateChanged;
            responseHandlersCollection.Add(sessionHandler.GetFullAPIPath(), sessionHandler);

            // device mode status
            SystemModeStatusResponseHandler systemModeStatusHandler = new SystemModeStatusResponseHandler(apiSession, APIGlobal.API_SYSTEM_MODE_STATUS);
            systemModeStatusHandler.OnUnknownResponseReceived += ResponseHandlerUnhandledResponseReceived;
            systemModeStatusHandler.OnErrorResponseReceived += ReponseHandlerServerErrorResponseReceived;
            systemModeStatusHandler.OnSystemModeChanged += SystemModeStatusChanged;
            responseHandlersCollection.Add(systemModeStatusHandler.GetFullAPIPath(), systemModeStatusHandler);

            // add additional handlers to collection
        } 
        #endregion





        #region Class Functions
        /// <summary>
        /// handle reconnecting to the device
        /// </summary>
        private void SessionReconnect( )
        {
            SessionReconnect(SESSION_DEFAULT_TIME, SESSION_EXTEND_TIME);
        }

        private void SessionReconnect(int startTime, int repeatTime)
        {
            SetSessionState(eSessionState.LOGGING_IN, false);

            CCLDebug.PrintToConsoleAndLog(eDebugLevel.Trace, "{0}.SessionReconnect( ) : Reconnecting to {1}", this.GetType().Name, apiSession.HostnameOrIPAddress);

            statusPollingTimer.Stop();
            sendCommandProcessingQueue.Clear();

            if (apiSession.ConnectionRequested == true)
                loginTimer.Reset(startTime, repeatTime);   

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandName"></param>
        /// <returns></returns>
        private APICommand GetCommand(string commandName)
        {
            APICommand command = null;
            apiCommandCollection.TryGetValue(commandName, out command);
            return command;
        }

        // lookup and send the command with the given name
        private void SendCommand(string commandName)
        {
            APICommand command = GetCommand(commandName);
            if (command != null)
                SendCommand(command);
        }

        // send the specified command by adding to the queue
        private void SendCommand(APICommand command)
        {
            sendCommandProcessingQueue.Enqueue(command);
        }

        /// <summary>
        /// Add all status polling commands to the queue to send
        /// </summary>
        private void EnqueueStatusPollingCommands()
        {            
            sendCommandProcessingQueue.Enqueue(GetCommand(APIGlobal.API_SYSTEM_MODE_STATUS));
            //PrintDebugMessage("{0}.EnqueueStatusPollingCommands( ) : Adding Commands to Queue[{1}]", this.GetType().Name, sendCommandProcessingQueue.Count);
        }

        private void SetSessionState(eSessionState state, bool connected )
        {
            apiSession.Connected = connected;
            SetSessionState(state);
        }

        private void SetSessionState(eSessionState state)
        {
            apiSession.State = state;
            UpdateSessionStateFeedback();
        }

        private void UpdateSessionStateFeedback()
        {
            // notify listeners
            if (OnSessionStateChanged != null)
                CrestronInvoke.BeginInvoke(thread => OnSessionStateChanged(this, new SessionStateChangedEventArgs(apiSession)));
        }

        private void UpdateErrorFeedback(bool isError, string message)
        {
            if (OnErrorStateChanged != null)
                CrestronInvoke.BeginInvoke(thread => OnErrorStateChanged(this, new ErrorChangeEventArgs(isError, message)));
        }

        private void ClearErrorFeedback()
        {
            UpdateErrorFeedback(false, "");
        }
        #endregion




        #region Control System Event handlers
        /// <summary>
        /// Called when the program status changes
        /// </summary>
        /// <param name="programStatusEvent"></param>
        private void PolyVideoOSDevice_ProgramStatusEventHandler(eProgramStatusEventType programStatusEvent)
        {
            switch (programStatusEvent)
            {
                case eProgramStatusEventType.Stopping:

                    // program is stopping, disconnect and dispose of the server
                    Disconnect();
                    Dispose();
                    break;

                case eProgramStatusEventType.Paused:
                    break;

                case eProgramStatusEventType.Resumed:
                    //if (apiSession.HasConnectedBefore)
                    //   Connect();
                    break;
            }
        }

        /// <summary>
        /// Called when the status of the network changes
        /// </summary>
        /// <param name="ethernetEventArgs"></param>
        private void PolyVideoOSDevice_EthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            // when the network link goes up reconnect if the server has already been connected
            switch (ethernetEventArgs.EthernetEventType)
            {
                case eEthernetEventType.LinkUp:
                    // ethernet link whent up, reconnect as needed
                    if (apiSession.ConnectionRequested)
                    {                        
                        Connect();
                    }
                    break;

                case eEthernetEventType.LinkDown:
                    // ethernet link went down, disconnect or do nothing?
                    CCLDebug.PrintToConsoleAndLog(eDebugLevel.Notice, "{0} - Ethernet Link Disconnected", this.GetType().Name);

                    loginTimer.Stop();
                    statusPollingTimer.Stop();

                    sendCommandProcessingQueue.Clear();
                    responseHandlerProcessingQueue.Clear();
                    break;
            }
        } 
        #endregion





        #region Debuggin Functions
        private void PrintDebugMessage(string msg)
        {
            if (this.debugState)
                CrestronConsole.PrintLine(msg);
        }

        private void PrintDebugMessage(string msg, params object[] args)
        {
            if (this.debugState)
                CrestronConsole.PrintLine(String.Format(msg, args));
        }

        private void PrintDebugMessage(Exception e)
        {
            if (this.debugState)
            {
                CrestronConsole.PrintLine("******* Exception *********");
                CrestronConsole.PrintLine("Message : {0} ", e.Message);
                CrestronConsole.PrintLine("Stacktrace : {0}", e.StackTrace);

                if (e.InnerException != null)
                    PrintDebugMessage(e.InnerException);
                else
                    CrestronConsole.PrintLine("********** END *************");
            }
        }

        private void PrintDebugMessage(CCLWebResponse response)
        {
            if (this.debugState)
            {
                CrestronConsole.PrintLine("******** Response ************");
                CrestronConsole.PrintLine("Response URL = {0}", response.ResponseURL);
                CrestronConsole.PrintLine("Response Code = {0}", response.StatusCode);
                CrestronConsole.PrintLine("Response Type = {0}", response.OriginalRequestType);
                CrestronConsole.PrintLine("Response Content = {0}", response.Content);
                PrintDebugMessage(response.Headers);
            }
        }

        private void PrintDebugMessage(Dictionary<string, string> dictionary)
        {
            if (this.debugState)
            {
                StringBuilder str = new StringBuilder();

                str.Append("\n*********** DICTIONARY **************");

                if (dictionary == null)
                    str.Append("\nEMPTY");
                else
                {
                    foreach (KeyValuePair<string, string> data in dictionary)
                    {
                        str.Append("\n " + data.Key + " = ");
                        if (data.Value == null)
                            str.Append("NULL");
                        else
                            str.Append(data.Value);
                    }
                }

                str.Append("\n*********** END **************");

                CrestronConsole.Print(str.ToString());
            }
        }

        /// <summary>
        /// Print out the state of the connection.
        /// </summary>
        /// <returns></returns>
        public void PrintDebugState()
        {
            CrestronConsole.PrintLine("                             : {0}", this.GetType().Name);
            CrestronConsole.PrintLine("");

            CrestronConsole.PrintLine("Login Timer Disposed         : {0}", loginTimer.Disposed);
            CrestronConsole.PrintLine("Polling Timer Disposed       : {0}", statusPollingTimer.Disposed);
            CrestronConsole.PrintLine("Command Queue Length         : {0}", sendCommandProcessingQueue.Count);
            CrestronConsole.PrintLine("Response Queue Length        : {0}", responseHandlerProcessingQueue.Count);

            apiSession.PrintDebugState();
            responseHandlerProcessingQueue.PrintDebugState();
            sendCommandProcessingQueue.PrintDebugState();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            try
            {       
                loginTimer.Stop();
                loginTimer.Dispose();
                statusPollingTimer.Stop();
                statusPollingTimer.Dispose();

                apiSession.Dispose();
                sendCommandProcessingQueue.Dispose();
                responseHandlerProcessingQueue.Dispose();

                if (responseHandlersCollection != null)
                {
                    foreach (KeyValuePair<string, APIResponseHandlerBase> kvp in responseHandlersCollection)
                    {
                        kvp.Value.Dispose();
                    }

                    responseHandlersCollection.Clear();
                }

                if (apiCommandCollection != null)
                {
                    foreach (KeyValuePair<string, APICommand> kvp in apiCommandCollection)
                    {
                        kvp.Value.Dispose();
                    }

                    apiCommandCollection.Clear();
                }
            }
            finally
            {
                apiSession = null;
                sendCommandProcessingQueue = null;
                responseHandlerProcessingQueue = null;
                responseHandlersCollection = null;
                apiCommandCollection = null;
                loginTimer = null;
                statusPollingTimer = null;

                _disposed = true;
            }

        }

        #endregion
    }
}
