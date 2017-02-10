using System;
using System.Collections.Generic;

namespace Dmarc.AggregateReport.Api.Handlers.Util
{
    internal static class DictionaryParameterExtensionMethods
    {
        public static DateTime? GetDateTime(this IDictionary<string, string> parameters, string name)
        {
            string dateUtcString;
            DateTime dateTime;
            return parameters.TryGetValue(name, out dateUtcString) && DateTime.TryParse(dateUtcString, out dateTime)
                ? dateTime
                : (DateTime?) null;
        }

        public static int? GetInt(this IDictionary<string, string> parameters, string name)
        {
            string intValueString;
            int intValue;
            return parameters.TryGetValue(name, out intValueString) && int.TryParse(intValueString, out intValue)
                ? intValue
                : (int?)null;
        }

        public static string GetString(this IDictionary<string, string> parameters, string name)
        {
            string domain;
            return parameters.TryGetValue(name, out domain)
                ? domain
                : null;
        }
    }
}