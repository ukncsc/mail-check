using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;

namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Utils
{
    public enum EntityHashType
    {
        [EnumDbName("MD5")]
        Md5 = 0,
        [EnumDbName("SHA-1")]
        Sha1 = 1
    }
}
