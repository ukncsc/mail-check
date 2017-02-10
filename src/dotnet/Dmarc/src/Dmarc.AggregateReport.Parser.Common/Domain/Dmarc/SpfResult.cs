namespace Dmarc.AggregateReport.Parser.Common.Domain.Dmarc
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