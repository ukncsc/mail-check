using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Dmarc.DnsRecord.Evaluator.Spf.ReadModel
{
    public class SpfConfig
    {
        public SpfConfig(List<SpfRecord> records, List<Error> errors, int totalErrorCount, ErrorType? maxErrorSeverity, DateTime lastChecked)
        {
            Records = records;
            Errors = errors;
            TotalErrorCount = totalErrorCount;
            MaxErrorSeverity = maxErrorSeverity;
            LastChecked = lastChecked;
        }

        public List<SpfRecord> Records { get; }
        public List<Error> Errors { get; }
        public int TotalErrorCount { get; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorType? MaxErrorSeverity { get; }
        public DateTime LastChecked { get; }
    }
}