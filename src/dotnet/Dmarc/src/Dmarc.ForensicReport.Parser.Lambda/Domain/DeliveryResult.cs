namespace Dmarc.ForensicReport.Parser.Lambda.Domain
{
    public enum DeliveryResult
    {
        Delivered = 0, //rfc6591
        Spam = 1, //rfc6591
        Policy = 2, //rfc6591
        Reject = 3, //rfc6591
        Other = 4 //rfc6591
    }
}