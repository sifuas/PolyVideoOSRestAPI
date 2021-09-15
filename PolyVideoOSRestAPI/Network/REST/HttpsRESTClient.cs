using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// simpl# libraries
using Crestron.SimplSharp;
using Crestron.SimplSharp.Net.Https;

using PolyVideoOSRestAPI.Network;
using PolyVideoOSRestAPI.Logging;

using RequestType = Crestron.SimplSharp.Net.Https.RequestType;
using HttpsClient = Crestron.SimplSharp.Net.Https.HttpsClient;

namespace PolyVideoOSRestAPI.Network.REST
{
    /// <summary>
    /// Client to implement basic REST communication over HTTPS
    /// </summary>
    public class HttpsRESTClient : GenericRestClientBase, IDisposable, IDebuggable
    {
        public int PoolSize { get; private set; }

        // store connections                
        private readonly ObjectPool<HttpsClient> _httpsClientPool;

        /// <summary>
        /// Default constructor for simpl+
        /// </summary>
        public HttpsRESTClient() : this(10)
        {            
        }

        /// <summary>
        /// Construct the client with the given pool size
        /// </summary>
        /// <param name="poolsize"></param>
        public HttpsRESTClient(int poolsize)
        {
            PoolSize = poolsize;
            _httpsClientPool
                        = new ObjectPool<HttpsClient>(PoolSize, PoolSize,
                            () => new HttpsClient
                            {
                                PeerVerification = false,
                                HostVerification = false,
                                TimeoutEnabled = true,
                                Timeout = 5,
                                KeepAlive = false
                            }) { CleanupPoolOnDispose = true };
        }

        /// <summary>
        /// Submit the HTTPS request with the given parameters and return the data from the response.
        /// Currently only uses basic authentication
        /// </summary>
        /// <param name="webRequest">The request data for the connection</param>
        /// <returns>The web response if one was received</returns>
        public override WebResponse SubmitRequest( WebRequest webRequest )
        {
            // validate data
            if ((webRequest.Host == null) || webRequest.Host.Length == 0)            
                throw new ArgumentException("Host / IP Address cannot be empty");            
            
            // get a client to use for the connection
            HttpsClient client = _httpsClientPool.GetFromPool();

            try
            {
                // verify that the client isn't already connected
                if (client.ProcessBusy)
                    client.Abort();

                Debug.PrintToConsole(eDebugLevel.Trace,"{0}.SubmitRequest() - Sending REST Request {1}", this.GetType().Name, webRequest);

                // configure client
                client.HostVerification = webRequest.HostVerification;
                client.PeerVerification = webRequest.PeerVerification;
                client.TimeoutEnabled   = webRequest.TimeoutEnabled;
                client.Timeout          = webRequest.Timeout;                
                
                if( webRequest.Headers != null )
                    client.IncludeHeaders   = true;            
              
                // create the initial request object with any initial values
                HttpsClientRequest httpsRequest = new HttpsClientRequest();
                
                httpsRequest.KeepAlive      = webRequest.KeepAlive;
                httpsRequest.RequestType    = webRequest.Type;
                httpsRequest.Encoding       = webRequest.EncodingType; ;

                // set the content of the request if there is any. GET request
                if (webRequest.Content != null)
                {
                    // set the source for the content to the appropriate type. in this case we are using a string but could be changed to a stream or byte[] array
                    httpsRequest.ContentSource = ContentSource.ContentString;
                    httpsRequest.ContentString = webRequest.Content;
                }
                else if( webRequest.ContentBytes != null )
                {
                    httpsRequest.ContentSource = ContentSource.ContentBytes;
                    httpsRequest.ContentBytes = webRequest.ContentBytes;
                }

                // add any header information to the request. this could include content-length, content-type, cookies, etc...
                if (webRequest.Headers != null)
                {
                    foreach (var header in webRequest.Headers)
                    {
                        httpsRequest.Header.AddHeader(new HttpsHeader(header.Key, header.Value));
                    }
                }

                // check if authentication is needed
                if ( ( webRequest.AuthenticationType != RequestAuthType.None ) && !String.IsNullOrEmpty(webRequest.Username))
                {
                    string authorizationHeader = NetworkHelperFunctions.CreateBasicAuthentiationBase64Encoding(webRequest.Username, webRequest.Password);

                    if (!String.IsNullOrEmpty(authorizationHeader))
                    {
                        httpsRequest.Header.SetHeaderValue("Authorization", authorizationHeader);
                    }
                }

                // add the URL to the request
                httpsRequest.Url.Parse(base.GenerateURL(webRequest, true));                

                // send the request to the server and get the response back
                HttpsClientResponse response = client.Dispatch(httpsRequest);                

                Dictionary<string, string> responseHeaders = new Dictionary<string, string>();
                foreach( HttpsHeader header in response.Header )
                {
                    responseHeaders.Add(header.Name, header.Value);
                }

                // return the response data
                return new WebResponse((HttpStatusCode)response.Code, response.ContentString, response.ResponseUrl, responseHeaders, httpsRequest.RequestType );
            }
            finally
            {
                client.Abort();
                _httpsClientPool.AddToPool(client);
            }            
        }

        /// <summary>
        /// Print the state of this object
        /// </summary>
        public void PrintDebugState( )
        {
            CrestronConsole.PrintLine("{0} State", this.GetType().Name);            
            _httpsClientPool.PrintDebugState();
        }

        private void PrintRequestInfo(HttpsClientRequest request )
        {
            CrestronConsole.PrintLine("************   HttpsRESTClient Request Info ***************");
            CrestronConsole.PrintLine("Type = {0}", request.RequestType);
            CrestronConsole.PrintLine("Content = {0}", request.ContentString);
            CrestronConsole.PrintLine("URL = {0}", request.Url.ToString());                        

            foreach (HttpsHeader header in request.Header)
            {
                CrestronConsole.PrintLine("Header {0} = {1}", header.Name, header.Value);
            }
        }

        private void PrintResponseInfo(HttpsClientResponse response)
        {
            CrestronConsole.PrintLine("************   HttpsRESTClient Response Info ( ) ***************" );
            CrestronConsole.PrintLine( "Respone Code = {0}", response.Code);            
            CrestronConsole.PrintLine( "Response URL = {0}", response.ResponseUrl);
            CrestronConsole.PrintLine( "Response Content = {0}", response.ContentString);     

            foreach( HttpsHeader header in response.Header )
            {
                CrestronConsole.PrintLine("Header {0} = {1}", header.Name, header.Value);
            }
            
        }

        /// <summary>
        /// Release any used resources
        /// </summary>
        void IDisposable.Dispose()
        {
            if (_httpsClientPool != null)
                _httpsClientPool.Dispose();
        }
    }
}