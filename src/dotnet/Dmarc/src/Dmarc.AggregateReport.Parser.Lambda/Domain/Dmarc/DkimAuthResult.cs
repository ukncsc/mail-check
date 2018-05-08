namespace Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc
{
    public class DkimAuthResult
    {
        public DkimAuthResult()
        {
        }

        public DkimAuthResult(string domain, DkimResult? result, string humanResult)
        {
            Domain = domain;
            Result = result;
            HumanResult = humanResult;
        }

        public string Domain { get; set; }

        public DkimResult? Result { get; set; }

        public string HumanResult { get; set; }
    }
}