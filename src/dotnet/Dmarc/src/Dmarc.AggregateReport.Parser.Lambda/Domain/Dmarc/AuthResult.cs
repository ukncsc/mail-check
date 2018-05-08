namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
{
    public class AuthResult
    {
        public AuthResult()
        {
        }

        public AuthResult(DkimAuthResult[] dkim, SpfAuthResult[] spf)
        {
            Dkim = dkim;
            Spf = spf;
        }

        public DkimAuthResult[] Dkim { get; set; }

        public SpfAuthResult[] Spf { get; set; }
    }
}