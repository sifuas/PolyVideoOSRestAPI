using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace PolyVideoOSRestAPI.API_Objects
{
    /// <summary>
    /// Class representing an object that can be return from the API
    /// </summary>
    public abstract class APIObjectBase
    {
        private string _content = "";
        public string OriginalContent
        {
            get
            {
                return (_content == null) ? "" : _content;
            }
            set
            {
                _content = ((_content == null) ? "" : value);
            }
        }
    }
}