using System;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Domain
{
    public class Aspf : OptionalDefaultTag
    {
        public static Aspf Default = new Aspf("aspf=r", AlignmentType.R, true);

        public Aspf(string value, AlignmentType alignmentType, bool isImplicit=false)
            : base(value, isImplicit)
        {
            AlignmentType = alignmentType;
        }

        public AlignmentType AlignmentType { get; }

        public override string ToString()
        {
            return $"{base.ToString()},{Environment.NewLine}{nameof(AlignmentType)}: {AlignmentType}";
        }
    }
}