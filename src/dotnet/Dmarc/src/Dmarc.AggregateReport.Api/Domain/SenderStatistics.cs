namespace Dmarc.AggregateReport.Api.Domain
{
    public class SenderStatistics
    {
        public SenderStatistics(string ipAddress, int trustedCount, int dkimNoSpfCount, int spfNoDkimCount, int untrustedCount)
        {
            IpAddress = ipAddress;
            TrustedCount = trustedCount;
            DkimNoSpfCount = dkimNoSpfCount;
            SpfNoDkimCount = spfNoDkimCount;
            UntrustedCount = untrustedCount;
        }

        public string IpAddress { get; }
        public int TrustedCount { get; }
        public int DkimNoSpfCount { get; }
        public int SpfNoDkimCount { get; }
        public int UntrustedCount { get; }
    }
}