namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
{
    public enum PolicyOverride
    {
        forwarded,
        sampled_out,
        trusted_forwarder,
        other
    }
}