
namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
{
    public class Identifier
    {
        public Identifier()
        {
        }

        public Identifier(string envelopeTo, string envelopeFrom, string headerFrom)
        {
            EnvelopeTo = envelopeTo;
            EnvelopeFrom = envelopeFrom;
            HeaderFrom = headerFrom;
        }

        public string EnvelopeTo { get; set; }

        public string EnvelopeFrom { get; set; }

        public string HeaderFrom { get; set; }
    }
}