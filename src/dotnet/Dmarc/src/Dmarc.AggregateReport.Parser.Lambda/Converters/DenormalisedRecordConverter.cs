using System.Collections.Generic;
using System.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Utils.Conversion;

namespace Dmarc.AggregateReport.Parser.Lambda.Converters
{
    public interface IDenormalisedRecordConverter
    {
        List<DenormalisedRecord> ToDenormalisedRecord(Domain.Dmarc.AggregateReport aggregateReport, string originalUri);
    }

    public class DenormalisedRecordConverter : IDenormalisedRecordConverter
    {
        public List<DenormalisedRecord> ToDenormalisedRecord(Domain.Dmarc.AggregateReport aggregateReport, string originalUri)
        {
            return aggregateReport.Records.Select(record => new DenormalisedRecord(
                originalUri,
                aggregateReport.ReportMetadata?.OrgName,
                aggregateReport.ReportMetadata?.Email,
                aggregateReport.ReportMetadata?.ExtraContactInfo,
                ConversionUtils.UnixTimeStampToDateTime(aggregateReport.ReportMetadata.Range.Begin),
                ConversionUtils.UnixTimeStampToDateTime(aggregateReport.ReportMetadata.Range.End),
                aggregateReport.PolicyPublished?.Domain,
                aggregateReport.PolicyPublished?.Adkim,
                aggregateReport.PolicyPublished?.Aspf,
                aggregateReport.PolicyPublished.P,
                aggregateReport.PolicyPublished?.Sp,
                aggregateReport.PolicyPublished?.Pct,
                record.Row?.SourceIp,
                record.Row.Count,
                record.Row?.PolicyEvaluated?.Disposition,
                record.Row?.PolicyEvaluated?.Dkim,
                record.Row.PolicyEvaluated.Spf,
                record.Row?.PolicyEvaluated?.Reasons != null ? string.Join(",", record.Row?.PolicyEvaluated?.Reasons.Select(_ => _.PolicyOverride.ToString())) : null,
                record.Row?.PolicyEvaluated?.Reasons != null ? string.Join(",", record.Row?.PolicyEvaluated?.Reasons.Where(_ => _.Comment != null).Select(_ => _.Comment.ToString())) : null,
                record.Identifiers?.EnvelopeTo,
                record.Identifiers?.HeaderFrom,
                record.AuthResults?.Dkim != null ? string.Join(",", record.AuthResults?.Dkim.Where(_ => _.Domain != null).Select(_ => _.Domain)) : null,
                record.AuthResults?.Dkim != null ? string.Join(",", record.AuthResults?.Dkim.Select(_ => _.Result)) : null,
                record.AuthResults?.Dkim != null ? string.Join(",", record.AuthResults?.Dkim.Where(_ => _.HumanResult != null).Select(_ => _.HumanResult)) : null,
                record.AuthResults?.Spf != null ? string.Join(",", record.AuthResults?.Spf.Where(_ => _.Domain != null).Select(_ => _.Domain)) : null,
                record.AuthResults?.Spf != null ? string.Join(",", record.AuthResults?.Spf.Select(_ => _.Result)) : null)).ToList();
        }
    }
}
