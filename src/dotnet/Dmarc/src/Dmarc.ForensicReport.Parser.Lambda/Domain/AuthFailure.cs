namespace Dmarc.ForensicReport.Parser.Lambda.Domain
{
    public enum AuthFailure
    {
        Adsp = 0,
        BodyHash = 1,
        Revoked = 2,
        Signature = 3,
        Spf = 4,
        Dmarc = 5 //not to spec but have seen.
    }
}