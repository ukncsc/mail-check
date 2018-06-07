using System;
using System.Collections.Generic;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class MxTlsEvaluatorResults
    {
        public MxTlsEvaluatorResults(int id, string hostname, DateTime lastChecked, List<string> warnings, List<string> failures, List<string> inconclusives)
        {
            Id = id;
            Hostname = hostname;
            LastChecked = lastChecked;
            Warnings = warnings;
            Failures = failures;
            Inconclusives = inconclusives;
        }

        public MxTlsEvaluatorResults(int id, string hostname)
        {
            Id = id;
            Hostname = hostname;
            Warnings = new List<string>();
            Failures = new List<string>();
            Inconclusives = new List<string>();
        }

        public List<string> Failures { get; }

        public string Hostname { get; }

        public int Id { get; }

        public List<string> Inconclusives { get; }

        public DateTime LastChecked { get; }

        public List<string> Warnings { get; }
    }
}
