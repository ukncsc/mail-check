namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal enum PolicyOverride
    {
        forwarded,
        sampled_out,
        trusted_forwarder,
        other,
        localpolicy
    }
}