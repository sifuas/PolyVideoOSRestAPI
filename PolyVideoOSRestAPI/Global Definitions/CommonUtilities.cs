using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace PolyVideoOSRestAPI
{
    public static class CommonUtilities
    {
        /// <summary>
        /// Convert the given enumerable list of KeyValuePairs into a string
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static string ConvertToString( IEnumerable<KeyValuePair<string,string>> enumerable )
        {
            StringBuilder str = new StringBuilder();

            foreach (KeyValuePair<string,string> kvp in enumerable)
            {
                str.Append(kvp.Key + " = ");
                if (kvp.Value != null)
                    str.Append(kvp.Value);
                else
                    str.Append("NULL");
            }

            return str.ToString();
        }
    }
}