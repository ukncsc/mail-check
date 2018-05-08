namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
{
    public class Record
    {
        public Record()
        {
        }

        public Record(Row row, Identifier identifiers, AuthResult authResults)
        {
            Row = row;
            Identifiers = identifiers;
            AuthResults = authResults;
        }

        public Row Row { get; set; }

        public Identifier Identifiers { get; set; }

        public AuthResult AuthResults { get; set; }
    }
}