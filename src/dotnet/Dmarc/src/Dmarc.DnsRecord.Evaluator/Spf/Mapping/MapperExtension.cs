using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Mapping;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using DomainSpfConfig = Dmarc.DnsRecord.Evaluator.Spf.Domain.SpfConfig;
using DomainSpfRecord = Dmarc.DnsRecord.Evaluator.Spf.Domain.SpfRecord;
using DomainVersion = Dmarc.DnsRecord.Evaluator.Spf.Domain.Version;
using DomainTerm = Dmarc.DnsRecord.Evaluator.Spf.Domain.Term;
using Error = Dmarc.DnsRecord.Evaluator.Spf.ReadModel.Error;
using ErrorType = Dmarc.DnsRecord.Evaluator.Spf.ReadModel.ErrorType;
using SpfConfig = Dmarc.DnsRecord.Evaluator.Spf.ReadModel.SpfConfig;
using SpfRecord = Dmarc.DnsRecord.Evaluator.Spf.ReadModel.SpfRecord;
using Term = Dmarc.DnsRecord.Evaluator.Spf.ReadModel.Term;
using Version = Dmarc.DnsRecord.Evaluator.Spf.ReadModel.Version;

namespace Dmarc.DnsRecord.Evaluator.Spf.Mapping
{
    public static class MapperExtension
    {
        public static SpfConfig ToSpfConfig(this DomainSpfConfig domainSpfConfig)
        {
            List<SpfRecord> spfRecords = domainSpfConfig.Records.Select(ToSpfRecord).ToList();

            List<Error> errors = domainSpfConfig.Errors.Select(_ => _.ToError("Global"))
               .Concat(domainSpfConfig.Records.SelectMany((v, i) => v.AllErrors.Select(_ => _.ToError($"Record {i + 1}"))))
               .ToList();

            ErrorType? maxErrorSeverity = errors.Any()
                ? errors.Min(_ => _.ErrorType)
                : (ErrorType?)null;

            return new SpfConfig(spfRecords, errors, domainSpfConfig.AllErrorCount, maxErrorSeverity, domainSpfConfig.LastChecked);
        }

        public static SpfRecord ToSpfRecord(this DomainSpfRecord domainSpfRecord, int index)
        {
            Version version = domainSpfRecord.Version.ToVersion();
            List<Term> terms = domainSpfRecord.Terms.Select(_ => _.ToTerm()).ToList();
            return new SpfRecord(index + 1, version, terms);
        }

        public static Version ToVersion(this DomainVersion domainVersion)
        {
            List<Error> errors = domainVersion.AllErrors.Select(_ => _.ToError("Version")).ToList();

            return new Version(domainVersion.Value, domainVersion.Explanation, errors);
        }

        public static Term ToTerm(this DomainTerm domainTerm)
        {
            OptionalDefaultMechanism optionalDefaultMechanism = domainTerm as OptionalDefaultMechanism;
            bool isImplicit = optionalDefaultMechanism != null && optionalDefaultMechanism.IsImplicit;   

            List<Error> errors = domainTerm.AllErrors.Select(_ => _.ToError("Term")).ToList();

            return new Term(domainTerm.Value, domainTerm.Explanation, isImplicit, errors);
        }

        public static Error ToError(this Evaluator.Rules.Error domainError, string scope)
        {
            return new Error(domainError.ErrorType.ToErrorType(), scope, domainError.Message);
        }

        public static ErrorType ToErrorType(this Evaluator.Rules.ErrorType domainErrorType)
        {
            return (ErrorType)domainErrorType;
        }
    }
}