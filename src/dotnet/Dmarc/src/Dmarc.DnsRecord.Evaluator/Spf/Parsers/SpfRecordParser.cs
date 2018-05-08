using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Explainers;
using Dmarc.DnsRecord.Evaluator.Implicit;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public interface ISpfRecordParser
    {
        SpfRecord Parse(string spfRecord, string domain);
    }

    public class SpfRecordParser : ISpfRecordParser
    {
        private const string Separator = " ";
        private readonly ISpfVersionParser _versionParser;
        private readonly ITermParser _termParser;
        private readonly IRuleEvaluator<SpfRecord> _spfRecordRulesEvaluator;
        private readonly IImplicitProvider<Term> _implicitTermProvider;
        private readonly IExplainer<Domain.Version> _versionExplainer;
        private readonly IExplainer<Term> _termExplainer;

        public SpfRecordParser(ISpfVersionParser versionParser,
            ITermParser termParser,
            IRuleEvaluator<SpfRecord> spfRecordRulesEvaluator,
            IImplicitProvider<Term> implicitTermProvider,
            IExplainer<Domain.Version> versionExplainer,
            IExplainer<Term> termExplainer)
        {
            _versionParser = versionParser;
            _termParser = termParser;
            _spfRecordRulesEvaluator = spfRecordRulesEvaluator;
            _implicitTermProvider = implicitTermProvider;
            _versionExplainer = versionExplainer;
            _termExplainer = termExplainer;
        }

        public SpfRecord Parse(string record, string domain)
        {
            if (string.IsNullOrEmpty(record))
            {
                return null;
            }

            string[] stringTokens = record.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);

            string versionToken = stringTokens.ElementAtOrDefault(0);
            Domain.Version version = _versionParser.Parse(versionToken);

            string explanation;
            if (_versionExplainer.TryExplain(version, out explanation))
            {
                version.Explanation = explanation;
            }

            List<Term> terms = stringTokens.Skip(1).Select(_termParser.Parse).ToList();
            terms = terms.Concat(_implicitTermProvider.GetImplicitValues(terms)).ToList();

            foreach (Term term in terms)
            {
                if (_termExplainer.TryExplain(term, out explanation))
                {
                    term.Explanation = explanation;
                }
            }

            SpfRecord spfRecord = new SpfRecord(record, version, terms, domain);
            spfRecord.AddErrors(_spfRecordRulesEvaluator.Evaluate(spfRecord));
            return spfRecord;
        }
    }
}
