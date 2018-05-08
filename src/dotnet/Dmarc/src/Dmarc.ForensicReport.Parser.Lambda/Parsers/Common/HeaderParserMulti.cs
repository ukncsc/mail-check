using System;
using System.Collections.Generic;
using System.Linq;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common
{
    public abstract class HeaderParserMulti<TItem, TList> : HeaderParser<TList>
        where TList : List<TItem>
    {
        protected override TList DoConvert(List<string> values, string fieldName, bool valueMandatory, bool parseMandatory)
        {
            if (valueMandatory && !values.Any())
            {
                throw new ArgumentException($"Expected {fieldName} to have at least 1 value but had none.");
            }

            return (TList)values
                .Select(_ => Convert(_, fieldName, parseMandatory))
                .Where(_ => _ != null)
                .ToList();
        }

        protected abstract TItem Convert(string value, string fieldName, bool parseMandatory);

        protected override TList OnFieldNotFoundReturnValue()
        {
            return (TList)new List<TItem>();
        }
    }
}