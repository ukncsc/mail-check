namespace Dmarc.AggregateReport.Parser.Common.Domain.Dmarc
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