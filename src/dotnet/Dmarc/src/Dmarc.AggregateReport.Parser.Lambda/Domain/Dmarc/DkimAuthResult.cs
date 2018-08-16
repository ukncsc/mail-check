namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
{
    public class DkimAuthResult
    {
        public DkimAuthResult()
        {
        }

        public DkimAuthResult(string domain, string selector, DkimResult? result, string humanResult)
        {
            Domain = domain;
            Selector = selector;
            Result = result;
            HumanResult = humanResult;
        }

        public string Domain { get; set; }

        public string Selector { get; set; }

        public DkimResult? Result { get; set; }

        public string HumanResult { get; set; }
    }
}