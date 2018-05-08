namespace Dmarc.ForensicReport.Parser.Lambda.Domain
{
    public enum FeedbackType
    {
        Abuse = 0, //rfc5965
        Fraud = 1, //rfc5965
        Other = 2, //rfc5965
        Virus = 3, //rfc5965
        AuthFailure = 4, //rfc6591
    }
}