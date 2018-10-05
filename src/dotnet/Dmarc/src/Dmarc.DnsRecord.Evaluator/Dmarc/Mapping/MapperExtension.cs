using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using DmarcConfig = Dmarc.DnsRecord.Evaluator.Dmarc.ReadModel.DmarcConfig;
using DmarcRecord = Dmarc.DnsRecord.Evaluator.Dmarc.ReadModel.DmarcRecord;
using DomainDmarcConfig = Dmarc.DnsRecord.Evaluator.Dmarc.Domain.DmarcConfig;
using DomainDmarcRecord = Dmarc.DnsRecord.Evaluator.Dmarc.Domain.DmarcRecord;
using DomainTag = Dmarc.DnsRecord.Evaluator.Dmarc.Domain.Tag;
using DomainError = Dmarc.DnsRecord.Evaluator.Rules.Error;
using DomainErrorType = Dmarc.DnsRecord.Evaluator.Rules.ErrorType;
using Error = Dmarc.DnsRecord.Evaluator.Dmarc.ReadModel.Error;
using ErrorType = Dmarc.DnsRecord.Evaluator.Dmarc.ReadModel.ErrorType;
using Tag = Dmarc.DnsRecord.Evaluator.Dmarc.ReadModel.Tag;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Mapping
{
    public static class MapperExtension
    {
        public static DmarcConfig ToDmarcConfig(this DomainDmarcConfig domainDmarcConfig)
        {
            List<DmarcRecord> dmarcRecords = domainDmarcConfig.Records.Select(ToDmarcRecord).ToList();

            List<Error> errors = domainDmarcConfig.Errors.Select(_ => _.ToError("Global"))
                .Concat(domainDmarcConfig.Records.SelectMany((v, i) =>
                    v.AllErrors.Select(_ => _.ToError($"Record {i + 1}"))))
                .ToList();

            ErrorType? maxErrorSeverity = errors.Any()
                ? errors.Min(_ => _.ErrorType)
                : (ErrorType?) null;

            return new DmarcConfig(dmarcRecords, errors, domainDmarcConfig.AllErrorCount, maxErrorSeverity,
                domainDmarcConfig.LastChecked, domainDmarcConfig.IsInherited ? domainDmarcConfig.OrgDomain : null);
        }

        public static DmarcRecord ToDmarcRecord(this DomainDmarcRecord domainDmarcRecord, int index)
        {
            List<Tag> tags = domainDmarcRecord.Tags.Select(_ => _.ToTag()).ToList();
            
            return new DmarcRecord(index + 1, tags);
        }

        public static Tag ToTag(this DomainTag domainTag)
        {
            OptionalDefaultTag optionalDefaultTag = domainTag as OptionalDefaultTag;
            bool isImplicit = optionalDefaultTag != null && optionalDefaultTag.IsImplicit;

            List<Error> errors = domainTag.AllErrors.Select(_ => _.ToError("Tag")).ToList();

            return new Tag(domainTag.Value, domainTag.Explanation, isImplicit, errors);
        }

        public static Error ToError(this DomainError domainError, string scope)
        {
            return new Error(domainError.ErrorType.ToErrorType(), scope, domainError.Message);
        }

        public static ErrorType ToErrorType(this DomainErrorType domainErrorType)
        {
            return (ErrorType)domainErrorType;
        }
    }
}
