namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal enum EntityPolicyOverride
    {
        forwarded,
        sampled_out,
        trusted_forwarder,
        other,
        localpolicy
    }
}