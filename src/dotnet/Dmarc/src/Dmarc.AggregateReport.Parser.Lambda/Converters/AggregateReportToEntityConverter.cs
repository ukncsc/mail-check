using System.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Dao.Entities;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Lambda.Utils.Conversion;
using Dmarc.Common.Report.Conversion;
using Alignment = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.Alignment;
using Disposition = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.Disposition;
using DkimAuthResult = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.DkimAuthResult;
using DkimResult = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.DkimResult;
using DmarcResult = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.DmarcResult;
using PolicyOverride = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.PolicyOverride;
using PolicyOverrideReason = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.PolicyOverrideReason;
using Record = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.Record;
using SpfAuthResult = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.SpfAuthResult;
using SpfResult = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.SpfResult;

namespace Dmarc.AggregateReport.Parser.Lambda.Converters
{
    internal class AggregateReportToEntityConverter : IToEntityConverter<AggregateReportInfo, AggregateReportEntity>
    {
        public AggregateReportEntity Convert(AggregateReportInfo aggregateReport)
        {
            return new AggregateReportEntity
            {
                RequestId = aggregateReport.EmailMetadata.RequestId,
                OrginalUri = aggregateReport.EmailMetadata.OriginalUri,
                AttachmentFilename = aggregateReport.AttachmentMetadata.Filename,
                Version = aggregateReport.AggregateReport.Version,
                OrgName = aggregateReport.AggregateReport.ReportMetadata?.OrgName,
                ReportId = aggregateReport.AggregateReport.ReportMetadata.ReportId,
                Email = aggregateReport.AggregateReport.ReportMetadata?.Email,
                ExtraContactInfo = aggregateReport.AggregateReport.ReportMetadata?.ExtraContactInfo,
                BeginDate = ConversionUtils.UnixTimeStampToDateTime(aggregateReport.AggregateReport.ReportMetadata.Range.Begin),
                EndDate = ConversionUtils.UnixTimeStampToDateTime(aggregateReport.AggregateReport.ReportMetadata.Range.End),
                EffectiveDate = ConversionUtils.UnixTimeStampToDateTime(aggregateReport.AggregateReport.ReportMetadata.Range.EffectiveDate),
                Domain = aggregateReport.AggregateReport.PolicyPublished?.Domain,
                Adkim = Convert(aggregateReport.AggregateReport.PolicyPublished?.Adkim),
                Aspf = Convert(aggregateReport.AggregateReport.PolicyPublished?.Aspf),
                P = Convert(aggregateReport.AggregateReport.PolicyPublished.P),
                Sp = Convert(aggregateReport.AggregateReport.PolicyPublished?.Sp),
                Pct = aggregateReport.AggregateReport.PolicyPublished?.Pct,
                Fo = aggregateReport.AggregateReport.PolicyPublished.Fo,
                Records = aggregateReport.AggregateReport.Records?.Select(ConvertToEntity).ToList()
            };
        }

        private EntityAlignment? Convert(Alignment? alignment)
        {
            return alignment.HasValue
                ? (EntityAlignment?) alignment.Value
                : null;
        }

        private EntityDisposition Convert(Disposition disposition)
        {
            return (EntityDisposition) disposition;
        }

        private EntityDisposition? Convert(Disposition? disposition)
        {
            return disposition.HasValue
                ? (EntityDisposition?)disposition.Value
                : null;
        }

        private RecordEntity ConvertToEntity(Record record)
        {
            return new RecordEntity
            {
                SourceIp = record.Row?.SourceIp,
                Count = record.Row.Count,
                Disposition = Convert(record.Row?.PolicyEvaluated?.Disposition),
                Dkim = Convert(record.Row?.PolicyEvaluated?.Dkim),
                Spf = Convert(record.Row.PolicyEvaluated.Spf),
                Reason = record.Row?.PolicyEvaluated?.Reasons?.Select(ConvertToEntity).ToList(),
                EnvelopeTo = record.Identifiers?.EnvelopeTo,
                EnvelopeFrom = record.Identifiers?.EnvelopeFrom,
                HeaderFrom = record.Identifiers?.HeaderFrom,
                DkimAuthResults = record.AuthResults?.Dkim?.Select(ConvertToEntity).ToList(),
                SpfAuthResults = record.AuthResults?.Spf?.Select(ConvertToEntity).ToList()
            };
        }

        private EntityDmarcResult? Convert(DmarcResult? dmarcResult)
        {
            return dmarcResult.HasValue
                ? (EntityDmarcResult?)dmarcResult.Value
                : null;
        }

        private PolicyOverrideReasonEntity ConvertToEntity(PolicyOverrideReason policyOverrideReason)
        {
            return new PolicyOverrideReasonEntity
            {
                PolicyOverride = Convert(policyOverrideReason.PolicyOverride),
                Comment = policyOverrideReason.Comment
            };
        }

        private EntityPolicyOverride? Convert(PolicyOverride? policyOverride)
        {
            return policyOverride.HasValue
                ? (EntityPolicyOverride?)policyOverride.Value
                : null;
        }

        private DkimAuthResultEntity ConvertToEntity(DkimAuthResult dkimAuthResult)
        {
            return new DkimAuthResultEntity
            { 
                Domain = dkimAuthResult.Domain,
                Selector = dkimAuthResult.Selector,
                Result = Convert(dkimAuthResult.Result),
                HumanResult = dkimAuthResult.HumanResult
            };
        }

        private EntityDkimResult? Convert(DkimResult? dkimResult)
        {
            return dkimResult.HasValue
                ? (EntityDkimResult?)dkimResult.Value
                : null;
        }

        private SpfAuthResultEntity ConvertToEntity(SpfAuthResult spfAuthResult)
        {
            return new SpfAuthResultEntity
            {
                Domain = spfAuthResult.Domain,
                Scope = Convert(spfAuthResult.Scope),
                Result =  Convert(spfAuthResult.Result)
            };
        }

        private EntitySpfResult? Convert(SpfResult? spfResult)
        {
            return spfResult.HasValue
                ? (EntitySpfResult?)spfResult.Value
                : null;
        }

        private EntitySpfDomainScope? Convert(SpfDomainScope? scope)
        {
            return scope.HasValue
                ? (EntitySpfDomainScope?) scope.Value
                : null;
        }
    }
}
