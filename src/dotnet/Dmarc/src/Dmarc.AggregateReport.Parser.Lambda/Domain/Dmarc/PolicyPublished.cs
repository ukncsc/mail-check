namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
{
    public class PolicyPublished
    {
        public PolicyPublished()
        {
        }

        public PolicyPublished(string domain, Alignment? adkim, Alignment? aspf, Disposition p, Disposition? sp, int? pct, string fo)
        {
            Domain = domain;
            Adkim = adkim;
            Aspf = aspf;
            P = p;
            Sp = sp;
            Pct = pct;
            Fo = fo;
        }

        public string Domain { get; set; }

        public Alignment? Adkim { get; set; }

        public Alignment? Aspf { get; set; }

        public Disposition P { get; set; }

        public Disposition? Sp { get; set; }

        public int? Pct { get; set; }

        public string Fo { get; set; }
    }
}