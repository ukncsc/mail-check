namespace Dmarc.AggregateReport.Parser.Common.Domain.Dmarc
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
    }
}