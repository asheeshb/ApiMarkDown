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
            if (src == null && stringArrayToCompare.Any(t => t == null))
                return true;

            if (src == string.Empty && stringArrayToCompare.Any(t => t == string.Empty))
                return true;

            foreach (string str in stringArrayToCompare)
            {
                if (str.Equals(str, strCmpType))
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
