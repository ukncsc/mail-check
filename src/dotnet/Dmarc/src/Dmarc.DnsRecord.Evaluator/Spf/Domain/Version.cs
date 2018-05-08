namespace Dmarc.DnsRecord.Evaluator.Spf.Domain
{
    public class Version : SpfEntity
    {
        public Version(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public string Explanation { get; set; }

        public override string ToString()
        {
            return $"{nameof(Value)}: {Value}";
        }
    }
}