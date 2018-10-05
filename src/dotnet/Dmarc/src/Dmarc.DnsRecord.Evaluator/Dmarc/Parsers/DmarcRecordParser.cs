using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Implict;
using Dmarc.DnsRecord.Evaluator.Explainers;
using Dmarc.DnsRecord.Evaluator.Implicit;
using Dmarc.DnsRecord.Evaluator.Rules;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Parsers
{
    public interface IDmarcRecordParser
    {
        bool TryParse(string record, string domain, string orgDomain, bool isTld, bool isInherited, out DmarcRecord dmarcRecord);
    }

    public class DmarcRecordParser : IDmarcRecordParser
    {
        private const char Separator = ';';
        private readonly ITagParser _tagParser;
        private readonly IRuleEvaluator<DmarcRecord> _ruleEvaluator;
        private readonly IImplicitProvider<Tag> _implicitProvider;
        private readonly IExplainer<Tag> _explainer;

        public DmarcRecordParser(ITagParser tagParser,
            IRuleEvaluator<DmarcRecord> ruleEvaluator,
            IImplicitProvider<Tag> implicitProvider,
            IExplainer<Tag> explainer)
        {
            _tagParser = tagParser;
            _ruleEvaluator = ruleEvaluator;
            _implicitProvider = implicitProvider;
            _explainer = explainer;
        }

        public bool TryParse(string record, string domain, string orgDomain, bool isTld, bool isInherited, out DmarcRecord dmarcRecord)
        {
            if (string.IsNullOrEmpty(record))
            {
                dmarcRecord = null;
                return false;
            }

            string[] stringTags = record.Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).ToArray();

            List<Tag> tags = _tagParser.Parse(stringTags.ToList());

            tags = tags.Concat(_implicitProvider.GetImplicitValues(tags)).ToList();

            foreach (Tag tag in tags)
            {
                string explanation;
                if (_explainer.TryExplain(tag, out explanation))
                {
                    tag.Explanation = explanation;
                }
            }

            dmarcRecord = new DmarcRecord(record, tags, domain, orgDomain, isTld, isInherited);
            dmarcRecord.AddErrors(_ruleEvaluator.Evaluate(dmarcRecord));
            return true;
        }
    }
}
