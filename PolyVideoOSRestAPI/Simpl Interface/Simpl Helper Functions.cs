using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace MEI.Integration.PolyVideoOSRestAPI.Simpl_Interface
{
    public static class SimplHelperFunctions
    {

        public static ushort UShortStringIsNullOrEmptry(string s)
        {
            return ConvertBooleanToUShort( String.IsNullOrEmpty(s) );
        }

        public static ushort ConvertBooleanToUShort(bool value)
        {
            return (ushort)(value ? 1 : 0);
        }

    }
}