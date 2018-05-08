using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Serialisation.AggregateReportDeserialisation
{
    public class TestConstants
    {
        public const string ExpectedDomain = "gov.uk";
        public const DkimResult ExpectedDkimResult = DkimResult.neutral;
        public const string ExpectedHumanResult = "humanresult";

        public const string ExpectedHeaderFrom = "gov.uk";
        public const string ExpectedEnvelopeTo = "gov.uk";

        public const Disposition ExpectedDisposition = Disposition.none;
        public const DmarcResult ExpectedDkimDmarcResult = DmarcResult.fail;
        public const DmarcResult ExpectedSpfDmarcResult = DmarcResult.fail;

        public const string ExpectedComment = "known forwarder";

        public const int ExpectedPct = 100;

        public const Alignment ExpectedAspfAlignment = Alignment.r;
        public const Alignment ExpectedAdkimAlignment = Alignment.r;

        public const string ExpectedOrgName = "Provider";
        public const string ExpectedEmail = "dmarc@provider.com";
        public const string ExpectedExtraContactInfo = "dmarcinfo@provider.com";
        public const string ExpectedReportId = "14764962785345167802";
        public const int ExpectedRangeBegin = 1476403200;
        public const int ExpectedRangeEnd = 1476489599;
        public const string ExpectedError = "badness";
        public const string ExpectedError2 = "badness2";

        public const string ExpectedIpAddress = "192.168.1.1";
        public const int ExpectedCount = 40;

        public const SpfResult ExpectedSpfResult = SpfResult.temperror;

    }
}
