using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public class AllMechanismParser : IMechanismParserStrategy
    {
        //"all"
        public Term Parse(string mechanism, Qualifier qualifier, string arguments)
        {
            All all = new All(mechanism, qualifier);

            if (!string.IsNullOrEmpty(arguments))
            {
                string errorMessage = string.Format(SpfParserResource.InvalidValueErrorMessage, Mechanism, mechanism);
                all.AddError(new Error(ErrorType.Error, errorMessage));
            }

            return all;
        }

        public string Mechanism => "all";
    }
}