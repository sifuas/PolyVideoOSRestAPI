using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace MEI.Integration.PolyVideoOSRestAPI
{
    /// <summary>
    /// Interface to enforce a key parameter on all objects for lookup
    /// </summary>
    public interface IKeyedObject
    {
        // string key that must be unique to all objects in a system
        [JsonProperty("Key")]
        string Key { get; }
    }

}