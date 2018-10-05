using System;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.PublicSuffix;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Explainers;
using Dmarc.DnsRecord.Evaluator.Dmarc.Implict;
using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record;
using Dmarc.DnsRecord.Evaluator.Explainers;
using Dmarc.DnsRecord.Evaluator.Implicit;
using Dmarc.DnsRecord.Evaluator.Rules;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Evaluator.Test.Dmarc.Parsers
{
    [TestFixture]
    [Category("Component")]
    public class DmarcRecordParserComponentTest
    {
        private IDmarcRecordParser _dmarcRecordParser;

        [SetUp]
        public void SetUp()
        {
            _dmarcRecordParser = CreateDmarcRecordParser();
        }

        [Test]
        public void DmarcRecordWithDuplicateRuaTagsGeneratesCorrectError()
        {
            string rawDmarcRecord =
                "v=DMARC1; fo=1; p=quarantine; sp=quarantine; rua=mailto:dcc-dmarc@abc.gov.uk; rua=mailto:dmarc-rua@dmarc.service.gov.uk; ruf=mailto:dmarc-rua@dmarc.service.gov.uk; ruf=mailto:dcc-dmarc@abc.gov.uk; pct=100";

            DmarcRecord dmarcRecord;
            bool parsed = _dmarcRecordParser.TryParse(rawDmarcRecord, "ncsc.gov.uk", string.Empty, false,false, out dmarcRecord);

            Assert.That(parsed, Is.True);
            Assert.That(dmarcRecord.AllErrorCount, Is.EqualTo(2));
            Assert.That(dmarcRecord.AllErrors[0].Message, Is.EqualTo("The rua tag should occur no more than once. This record has 2 occurrences."));
            Assert.That(dmarcRecord.AllErrors[1].Message, Is.EqualTo("The ruf tag should occur no more than once. This record has 2 occurrences."));
        }

        private IDmarcRecordParser CreateDmarcRecordParser()
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                .AddTransient<IDmarcRecordParser, DmarcRecordParser>()
                .AddTransient<ITagParser, TagParser>()
                .AddTransient<ITagParserStrategy, VersionParserStrategy>()
                .AddTransient<ITagParserStrategy, AdkimParserStrategy>()
                .AddTransient<ITagParserStrategy, AspfParserStrategy>()
                .AddTransient<ITagParserStrategy, FailureOptionsParserStrategy>()
                .AddTransient<ITagParserStrategy, PolicyParserStrategy>()
                .AddTransient<ITagParserStrategy, PercentParserStrategy>()
                .AddTransient<ITagParserStrategy, ReportFormatParserStrategy>()
                .AddTransient<ITagParserStrategy, ReportIntervalParserStrategy>()
                .AddTransient<ITagParserStrategy, ReportUriAggregateParserStrategy>()
                .AddTransient<ITagParserStrategy, ReportUriForensicParserStrategy>()
                .AddTransient<ITagParserStrategy, SubDomainPolicyParserStrategy>()
                .AddTransient<IUriTagParser, UriTagParser>()
                .AddTransient<IDmarcUriParser, DmarcUriParser>()
                .AddTransient<IMaxReportSizeParser, MaxReportSizeParser>()
                .AddTransient<IRuleEvaluator<DmarcRecord>, RuleEvaluator<DmarcRecord>>()
                .AddTransient<IRule<DmarcRecord>, VersionMustBeFirstTag>()
                .AddTransient<IRule<DmarcRecord>, MaxLengthOf450Characters>()
                .AddTransient<IRule<DmarcRecord>, PctValueShouldBe100>()
                .AddTransient<IRule<DmarcRecord>, PolicyShouldBeQuarantineOrReject>()
                .AddTransient<IRule<DmarcRecord>, PolicyTagMustExist>()
                .AddTransient<IRule<DmarcRecord>, RuaTagShouldNotHaveMoreThanTwoUris>()
                .AddTransient<IRule<DmarcRecord>, RuaTagsShouldBeMailTo>()
                .AddTransient<IRule<DmarcRecord>, RuaTagsShouldContainDmarcServiceMailBox>()
                .AddTransient<IRule<DmarcRecord>, RufTagShouldBeMailTo>()
                .AddTransient<IRule<DmarcRecord>, RufTagShouldNotHaveMoreThanTwoUris>()
                .AddTransient<IRule<DmarcRecord>, SubDomainPolicyShouldBeQuarantineOrReject>()
                .AddTransient<IImplicitProvider<Tag>, ImplicitProvider<Tag>>()
                .AddTransient<IImplicitProviderStrategy<Tag>, ReportIntervalImplicitProvider>()
                .AddTransient<IImplicitProviderStrategy<Tag>, ReportFormatImplicitProvider>()
                .AddTransient<IImplicitProviderStrategy<Tag>, PercentImplicitProvider>()
                .AddTransient<IImplicitProviderStrategy<Tag>, FailureOptionsImplicitProvider>()
                .AddTransient<IImplicitProviderStrategy<Tag>, AspfImplicitProvider>()
                .AddTransient<IImplicitProviderStrategy<Tag>, AdkimImplicitProvider>()
                .AddTransient<IImplicitProviderStrategy<Tag>, SubDomainPolicyImplicitProvider>()
                .AddTransient<IExplainer<Tag>, Explainer<Tag>>()
                .AddTransient<IExplainerStrategy<Tag>, AdkimExplainer>()
                .AddTransient<IExplainerStrategy<Tag>, AspfTagExplainer>()
                .AddTransient<IExplainerStrategy<Tag>, FailureOptionsExplainer>()
                .AddTransient<IExplainerStrategy<Tag>, PolicyExplainer>()
                .AddTransient<IExplainerStrategy<Tag>, PercentExplainer>()
                .AddTransient<IExplainerStrategy<Tag>, ReportFormatExplainer>()
                .AddTransient<IExplainerStrategy<Tag>, ReportIntervalExplainer>()
                .AddTransient<IExplainerStrategy<Tag>, ReportUriAggregateExplainer>()
                .AddTransient<IExplainerStrategy<Tag>, ReportUriForensicExplainer>()
                .AddTransient<IExplainerStrategy<Tag>, SubDomainPolicyExplainer>()
                .AddTransient<IExplainerStrategy<Tag>, VersionExplainer>()
                .AddSingleton<IOrganisationalDomainProvider, OrganisationDomainProvider>()
                .BuildServiceProvider();

            return serviceProvider.GetService<IDmarcRecordParser>();
        }
    }
}
