namespace Dmarc.DnsRecord.Evaluator.Dmarc.Domain
{
    public class FailureOption : OptionalDefaultTag
    {
        public static FailureOption Default = new FailureOption("fo=0", FailureOptionType.Zero, true);

        public FailureOption(string value, FailureOptionType failureOptionType, bool isImplicit = false) 
            : base(value, isImplicit)
        {
            FailureOptionType = failureOptionType;
        }

        public FailureOptionType FailureOptionType { get; }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(FailureOptionType)}: {FailureOptionType}";
        }
    }
}