namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
{
    public class SpfAuthResult
    {
        public SpfAuthResult()
        {
        }

        public SpfAuthResult(string domain, SpfDomainScope? scope, SpfResult? result)
        {
            Domain = domain;
            Scope = scope;
            Result = result;
        }

        public string Domain { get; set; }

        public SpfDomainScope? Scope { get; set; }

        public SpfResult? Result { get; set; }
    }
}