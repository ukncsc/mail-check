namespace Dmarc.DnsRecord.Evaluator.Dmarc.Domain
{
    public class Percent : OptionalDefaultTag
    {
        public static Percent Default = new Percent("pct=100", 100, true);

        public Percent(string value, int? percentValue, bool isImplicit = false) 
            : base(value, isImplicit)
        {
            PercentValue = percentValue;
        }

        public int? PercentValue { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(PercentValue)}: {PercentValue}";
        }
    }
}