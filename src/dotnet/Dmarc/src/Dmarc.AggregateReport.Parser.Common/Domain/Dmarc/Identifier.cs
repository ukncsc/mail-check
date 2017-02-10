
namespace Dmarc.AggregateReport.Parser.Common.Domain.Dmarc
{
    public class Identifier
    {
        public Identifier()
        {
        }

        public Identifier(string envelopeTo, string headerFrom)
        {
            EnvelopeTo = envelopeTo;
            HeaderFrom = headerFrom;
        }

        public string EnvelopeTo { get; set; }

        public string HeaderFrom { get; set; }
    }
}