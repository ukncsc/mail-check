using System;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;

namespace Dmarc.DnsRecord.Evaluator.Spf.Parsers
{
    public interface IQualifierParser
    {
        Qualifier Parse(string qualifier);
    }

    public class QualifierParser : IQualifierParser
    {
        public Qualifier Parse(string qualifier)
        {
            switch (qualifier)
            {
                case "":
                case "+":
                    return Qualifier.Pass;
                case "-":
                    return Qualifier.Fail;
                case "?":
                    return Qualifier.Neutral;
                case "~":
                    return Qualifier.SoftFail;
                default: 
                    throw new ArgumentException($"Unknown qualifier {qualifier}");
            }
        }
    }
}