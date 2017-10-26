using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bajra.Utils
{
    public static class Extenstions
    {
        public static bool IsEqualToAny(this string src, StringComparison strCmpType, params string[] stringArrayToCompare)
        {
            if (src == null)
                return stringArrayToCompare.Any(t => t == null);
            
            if (src == string.Empty)
                return stringArrayToCompare.Any(t => t == string.Empty);

            foreach (string str in stringArrayToCompare)
            {
                if (src.Equals(str, strCmpType))
                    return true;
            }

            return false;
        }

        public static bool IsEqualToAny(this string src, params string[] stringArrayToCompare)
        {
            return IsEqualToAny(src, StringComparison.InvariantCulture, stringArrayToCompare);
        }
    }
}
