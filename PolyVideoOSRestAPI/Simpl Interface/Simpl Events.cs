using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Crestron.SimplSharp;

using MEI.Integration.PolyVideoOSRestAPI;

namespace MEI.Integration.PolyVideoOSRestAPI.Simpl_Interface
{
    /// <summary>
    /// Encapsulate the state of the current session
    /// </summary>
    public class SessionStateChangedEventArgs : EventArgs
    {
        public string StringState {get; private set;}
        public eSessionState State {get; private set;}
        public bool Connected { get; private set; }

        public ushort UState {
            get
            {
                return ((ushort)State);
            }
        }

        public ushort UConnected
        {
            get
            {
                return(ushort)((Connected == true ) ? 1 : 0 );
            }
        }
        public SessionStateChangedEventArgs()
        {

        }

        internal SessionStateChangedEventArgs(APISession session )
        {
            if( session != null )
            {
                State = session.State;
                StringState = State.ToString();
                Connected = session.Connected;
            }
        }
    }
}