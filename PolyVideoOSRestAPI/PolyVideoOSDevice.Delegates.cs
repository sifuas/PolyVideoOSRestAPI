using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// crestron librarires
using Crestron.SimplSharp;

using MEI.Integration.PolyVideoOSRestAPI;
using MEI.Integration.PolyVideoOSRestAPI.API_Objects;

namespace MEI.Integration.PolyVideoOSRestAPI
{

    /// <summary>
    /// Delegate callback when the Session state changes
    /// </summary>
    /// <param name="sessionState"></param>
    /// <param name="state"></param>
    public delegate void ConnectionStateChangedDelegate( APISessionStateEventArgs args );

    /// <summary>
    /// Delegate callback when the System Mode changes.
    /// System mode is polled as long as a connection is active.
    /// </summary>
    /// <param name="systemMode"></param>
    public delegate void DeviceModeChangedDelegate( APISystemModeStateEventArgs args );

}