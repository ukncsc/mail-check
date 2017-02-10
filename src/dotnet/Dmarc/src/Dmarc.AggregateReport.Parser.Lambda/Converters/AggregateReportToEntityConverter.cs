using System.Linq;
using Dmarc.AggregateReport.Parser.Common.Domain;
using Dmarc.AggregateReport.Parser.Common.Domain.Dmarc;
using Dmarc.AggregateReport.Parser.Common.Utils.Conversion;
using AggregateReportEntity = Dmarc.AggregateReport.Parser.Lambda.Dao.Entities.AggregateReport;
using DkimAuthResultEntity = Dmarc.AggregateReport.Parser.Lambda.Dao.Entities.DkimAuthResult;
using EntityAlignment = Dmarc.AggregateReport.Parser.Lambda.Dao.Entities.Alignment;
using EntityDisposition = Dmarc.AggregateReport.Parser.Lambda.Dao.Entities.Disposition;
using EntityDkimResult = Dmarc.AggregateReport.Parser.Lambda.Dao.Entities.DkimResult;
using EntityDmarcResult = Dmarc.AggregateReport.Parser.Lambda.Dao.Entities.DmarcResult;
using EntityPolicyOverride = Dmarc.AggregateReport.Parser.Lambda.Dao.Entities.PolicyOverride;
using EntitySpfResult = Dmarc.AggregateReport.Parser.Lambda.Dao.Entities.SpfResult;
using PolicyOverrideReasonEntity = Dmarc.AggregateReport.Parser.Lambda.Dao.Entities.PolicyOverrideReason;
using RecordEntity = Dmarc.AggregateReport.Parser.Lambda.Dao.Entities.Record;
using SpfAuthResultEntity = Dmarc.AggregateReport.Parser.Lambda.Dao.Entities.SpfAuthResult;

namespace Dmarc.AggregateReport.Parser.Lambda.Converters
{
    internal interface IAggregateReportToEntityConverter
    {
        AggregateReportEntity ConvertToEntity(AggregateReportInfo aggregateReport);
    }

    internal class AggregateReportToEntityConverter : IAggregateReportToEntityConverter
    {
        public AggregateReportEntity ConvertToEntity(AggregateReportInfo aggregateReport)
        {
            return new AggregateReportEntity
            {
                RequestId = aggregateReport.EmailMetadata.RequestId,
                OrginalUri = aggregateReport.EmailMetadata.OriginalUri,
                AttachmentFilename = aggregateReport.AttachmentMetadata.Filename,
                OrgName = aggregateReport.AggregateReport.ReportMetadata?.OrgName,
                ReportId = aggregateReport.AggregateReport.ReportMetadata.ReportId,
                Email = aggregateReport.AggregateReport.ReportMetadata?.Email,
                ExtraContactInfo = aggregateReport.AggregateReport.ReportMetadata?.ExtraContactInfo,
                BeginDate = ConversionUtils.UnixTimeStampToDateTime(aggregateReport.AggregateReport.ReportMetadata.Range.Begin),
                EndDate = ConversionUtils.UnixTimeStampToDateTime(aggregateReport.AggregateReport.ReportMetadata.Range.End),
                Domain = aggregateReport.AggregateReport.PolicyPublished?.Domain,
                Adkim = Convert(aggregateReport.AggregateReport.PolicyPublished?.Adkim),
                Aspf = Convert(aggregateReport.AggregateReport.PolicyPublished?.Aspf),
                P = Convert(aggregateReport.AggregateReport.PolicyPublished.P),
                Sp = Convert(aggregateReport.AggregateReport.PolicyPublished?.Sp),
                Pct = aggregateReport.AggregateReport.PolicyPublished?.Pct,
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

        private EntityDmarcResult Convert(DmarcResult dmarcResult)
        {
            return (EntityDmarcResult) dmarcResult;
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
                Result =  Convert(spfAuthResult.Result)
            };
        }

        private EntitySpfResult? Convert(SpfResult? spfResult)
        {
            return spfResult.HasValue
                ? (EntitySpfResult?)spfResult.Value
                : null;
        }
    }
}
