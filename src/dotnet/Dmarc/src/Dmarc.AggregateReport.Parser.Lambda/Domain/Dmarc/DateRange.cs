namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
{
    public class DateRange
    {
        public DateRange()
        {
        }

        public DateRange(int begin, int end)
        {
            Begin = begin;
            End = end;
        }

        public int Begin { get; set; }

        public int End { get; set; }

        public int EffectiveDate => CalculateEffectiveDate();

        private int CalculateEffectiveDate()
        {
            int difference = End - Begin;
            int midPoint = (int)((double)difference / 2);
            return Begin + midPoint;
        }
    }
}