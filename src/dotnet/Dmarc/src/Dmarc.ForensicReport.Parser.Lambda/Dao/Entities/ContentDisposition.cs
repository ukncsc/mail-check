namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public enum ContentDisposition
    {
        [EnumDbName("inline")]
        Inline = 0,
        [EnumDbName("attachment")]
        Attachment = 1
    }
}