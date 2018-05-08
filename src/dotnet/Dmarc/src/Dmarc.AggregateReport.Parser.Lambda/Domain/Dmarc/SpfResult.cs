namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
{
    public enum SpfResult
    {
        none,
        neutral,
        pass,
        fail,
        softfail,
        temperror,
        permerror
    }
}