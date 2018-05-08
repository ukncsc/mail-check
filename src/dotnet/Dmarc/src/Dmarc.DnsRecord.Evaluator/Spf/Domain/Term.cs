using System;

namespace Dmarc.DnsRecord.Evaluator.Spf.Domain
{
    public abstract class Term : SpfEntity
    {
        protected Term(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public string Explanation { get; set; }

        public override string ToString()
        {
            string errorString = string.Join(Environment.NewLine, AllErrors);

            return $"{nameof(Value)}: {Value}{Environment.NewLine}" +
                   $"{nameof(Explanation)}: {Explanation}{Environment.NewLine}" +
                   $"{(AllValid ? "Valid" : $"Invalid{Environment.NewLine}{errorString}")}";
        }
    }
}