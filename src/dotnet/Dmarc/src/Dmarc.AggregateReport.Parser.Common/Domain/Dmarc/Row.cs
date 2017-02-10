namespace Dmarc.AggregateReport.Parser.Common.Domain.Dmarc
{
    public class Row
    {
        public Row()
        {
        }

        public Row(string sourceIp, int count, PolicyEvaluated policyEvaluated)
        {
            SourceIp = sourceIp;
            Count = count;
            PolicyEvaluated = policyEvaluated;
        }

        public string SourceIp { get; set; }

        public int Count { get; set; }

        public PolicyEvaluated PolicyEvaluated { get; set; }
    }
}