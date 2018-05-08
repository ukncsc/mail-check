using System;
using System.Text.RegularExpressions;

namespace Dmarc.DnsRecord.Importer.Lambda.Util
{
    public class StringUtils
    {
        public static bool SpaceInsensitiveEquals(string a, string b)
        {
            if ((object) a == (object) b)
            {
                return true;
            }
            if (a == null || b == null)
            {
                return false;
            }

            string aWithoutspaces = Regex.Replace(a, @"\s+", string.Empty, RegexOptions.IgnoreCase);
            string bWithoutspaces = Regex.Replace(b, @"\s+", string.Empty, RegexOptions.IgnoreCase);

            return string.Equals(aWithoutspaces, bWithoutspaces, StringComparison.OrdinalIgnoreCase);
        }
    }
}