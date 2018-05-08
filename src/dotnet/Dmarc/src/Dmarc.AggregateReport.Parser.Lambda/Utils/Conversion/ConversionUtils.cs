using System;

namespace Dmarc.AggregateReport.Parser.Lambda.Utils.Conversion
{
    public class ConversionUtils
    {
        public static int? StringToNullableInt(string stringValue)
        {
            int intValue;
            return int.TryParse(stringValue, out intValue) ?
                intValue :
                (int?)null;
        }

        public static DateTime UnixTimeStampToDateTime(int intDateTime)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(intDateTime);
            return dtDateTime;
        }
    }
}
