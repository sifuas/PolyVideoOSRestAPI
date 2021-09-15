using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace MEI.Integration.PolyVideoOSRestAPI
{
    public abstract class Global 
    {  
        // text and encodings

        // default encoding page for ISO-8859-1
        public static readonly int DEFAULT_ENCODING_CODEPAGE = 28591;        
        public static readonly string DEFAULT_ENCODING_NAME = "ISO-8859-1";
        
    }
}