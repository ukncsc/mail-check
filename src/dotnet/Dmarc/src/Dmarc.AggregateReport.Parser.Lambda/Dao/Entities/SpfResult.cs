namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal enum SpfResult
    {
        none,
        neutral,
        pass,
        fail,
        softfail,
        temperror,
        permerror,
        unknown
    }
}