namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
{
    public class PolicyPublished
    {
        public PolicyPublished()
        {
        }

        public PolicyPublished(string domain, Alignment? adkim, Alignment? aspf, Disposition p, Disposition? sp, int? pct)
        {
            Domain = domain;
            Adkim = adkim;
            Aspf = aspf;
            P = p;
            Sp = sp;
            Pct = pct;
        }

        public string Domain { get; set; }

        public Alignment? Adkim { get; set; }

        public Alignment? Aspf { get; set; }

        public Disposition P { get; set; }

        public Disposition? Sp { get; set; }

        public int? Pct { get; set; }
    }
}