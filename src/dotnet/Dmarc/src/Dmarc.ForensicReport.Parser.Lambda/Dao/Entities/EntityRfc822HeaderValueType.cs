namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public enum EntityRfc822HeaderValueType
    {
        [EnumDbName("text")]
        Text = 0,

        [EnumDbName("email")]
        Email = 1,

        [EnumDbName("ip")]
        Ip = 2,

        [EnumDbName("hostname")]
        Hostname = 3,

        [EnumDbName("date")]
        Date = 4
    }
}
