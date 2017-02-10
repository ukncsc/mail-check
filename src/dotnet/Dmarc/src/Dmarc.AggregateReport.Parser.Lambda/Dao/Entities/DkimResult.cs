namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal enum DkimResult
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