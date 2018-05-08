using System;
using System.Collections.Generic;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.Common
{
    public interface IHeaderParser<out T>
    {
        T Parse(Dictionary<string, List<string>> headerList, string fieldName, bool fieldMandatory = false, bool valueMandatory = false, bool parseMandatory = false);
    }

    public abstract class HeaderParser<T> : IHeaderParser<T>
    {
        public T Parse(Dictionary<string, List<string>> headerList, string fieldName, bool fieldMandatory, bool valueMandatory, bool parseMandatory)
        {
            List<string> values;
            if (headerList.TryGetValue(fieldName, out values))
            {
                return DoConvert(values, fieldName, valueMandatory, parseMandatory);
            }
            if (fieldMandatory)
            {
                throw new ArgumentException($"Expected to find {fieldName} in {nameof(headerList)} but didnt.");
            }
            return OnFieldNotFoundReturnValue();
        }

        protected abstract T DoConvert(List<string> values, string fieldName, bool valueMandatory, bool parseMandatory);

        protected virtual T OnFieldNotFoundReturnValue()
        {
            return default(T);
        }
    }
}