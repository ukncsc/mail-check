namespace Dmarc.AggregateReport.Parser.Common.Domain.Dmarc
{
    public class SpfAuthResult
    {
        public SpfAuthResult()
        {
        }

        public SpfAuthResult(string domain, SpfResult? result)
        {
            Domain = domain;
            Result = result;
        }

        public string Domain { get; set; }

        public SpfResult? Result { get; set; }
    }
}