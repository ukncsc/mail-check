using System.Text;

namespace Dmarc.DnsRecord.Importer.Lambda.Util
{
    public static class StringExtensionMethods
    {
        public static string EscapeNonAsciiChars(this string record)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in record.ToCharArray())
            {
                if (b < 32 || b > 127)
                {
                    sb.Append('\\');
                    sb.Append((int)b);
                }
                else
                {
                    sb.Append(b);
                }
            }

            return sb.ToString();
        }
    }
}
