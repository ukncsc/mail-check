namespace Dmarc.AggregateReport.Parser.Lambda.Dao.Entities
{
    internal enum EntitySpfResult
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