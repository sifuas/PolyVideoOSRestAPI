using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace PolyVideoOSRestAPI
{
    internal abstract class APIExceptionBase : Exception
    {
        public APIExceptionBase() { }
    }

}