namespace Dmarc.AggregateReport.Parser.Common.Domain.Dmarc
{
    public enum DkimResult
    {
        none,
        pass,
        fail,
        policy,
        neutral,
        temperror,
        permerror
    }
}