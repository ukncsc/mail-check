using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public interface ISpfConfigParser
    {
        SpfConfig Parse(Contract.Domain.SpfConfig spfConfig);
    }

    public class SpfConfigParser : ISpfConfigParser
    {
        private readonly ISpfRecordParser _spfRecordParser;
        private readonly IRuleEvaluator<SpfConfig> _spfConfigRulesEvaluator;

        public SpfConfigParser(ISpfRecordParser spfRecordParser,
            IRuleEvaluator<SpfConfig> spfConfigRulesEvaluator)
        {
            _spfRecordParser = spfRecordParser;
            _spfConfigRulesEvaluator = spfConfigRulesEvaluator;
        }

        public SpfConfig Parse(Contract.Domain.SpfConfig spfDomainConfig)
        {
            List<SpfRecord> records = spfDomainConfig.Records
                .Select(_ => _spfRecordParser.Parse(_, spfDomainConfig.Domain.Name))
                .Where(_ => _ != null).ToList();

            SpfConfig spfConfig = new SpfConfig(records, spfDomainConfig.LastChecked);

            List<Error> errors = _spfConfigRulesEvaluator.Evaluate(spfConfig);
            spfConfig.AddErrors(errors);

            return spfConfig;
        }
    }
}
