namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal enum EntityDkimResult
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