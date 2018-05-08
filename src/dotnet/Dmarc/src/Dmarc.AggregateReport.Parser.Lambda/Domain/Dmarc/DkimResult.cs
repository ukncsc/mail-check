namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
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