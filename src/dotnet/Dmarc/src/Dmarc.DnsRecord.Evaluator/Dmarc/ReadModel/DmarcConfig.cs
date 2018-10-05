using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.ReadModel
{
    public class DmarcConfig
    {
        public DmarcConfig(List<DmarcRecord> records, List<Error> errors, int totalErrorCount, ErrorType? maxErrorSeverity, DateTime lastChecked, string inheritedFromOrgName)
        {
            Records = records;
            Errors = errors;
            TotalErrorCount = totalErrorCount;
            MaxErrorSeverity = maxErrorSeverity;
            LastChecked = lastChecked;
            InheritedFrom = inheritedFromOrgName;
        }

        public List<DmarcRecord> Records { get; }
        public List<Error> Errors { get; }
        public int TotalErrorCount { get; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorType? MaxErrorSeverity { get; }
        public DateTime LastChecked { get; }
        public string InheritedFrom { get; }
    }
}
