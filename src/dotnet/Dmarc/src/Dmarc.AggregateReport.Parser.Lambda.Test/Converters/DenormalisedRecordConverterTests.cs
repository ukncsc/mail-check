using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Converters;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using NUnit.Framework;

namespace Dmarc.Lambda.AggregateReport.Parser.Test.Converters
{
    [TestFixture]
    public class DenormalisedRecordConverterTests
    {
        private DenormalisedRecordConverter _denormalisedRecordConverter;

        [SetUp]
        public void SetUp()
        {
            _denormalisedRecordConverter = new DenormalisedRecordConverter();
        }

        [Test]
        [TestCaseSource(nameof(CreateMappingTestData))]
        public void MappingTests<TOut>(TOut expected, Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.AggregateReport aggregateReport, Func<DenormalisedRecord, TOut> actualGetter)
        {
            IEnumerable<DenormalisedRecord> denormalisedRecords = _denormalisedRecordConverter.ToDenormalisedRecord(aggregateReport,string.Empty);
            Assert.That(expected, Is.EqualTo(actualGetter(denormalisedRecords.ElementAt(0))));
        }

        public static IEnumerable<TestCaseData> CreateMappingTestData()
        {
            yield return new TestCaseData("testOrgName", Create(a => a.ReportMetadata.OrgName = "testOrgName"), (Func<DenormalisedRecord, string>)(a => a.OrgName)).SetName("to denormalised record org name mapping test");
            yield return new TestCaseData("test@test.com", Create(a => a.ReportMetadata.Email = "test@test.com"), (Func<DenormalisedRecord, string>)(a => a.Email)).SetName("to denormalised record email address mapping test");
            yield return new TestCaseData("testContactInfo", Create(a => a.ReportMetadata.ExtraContactInfo = "testContactInfo"), (Func<DenormalisedRecord, string>)(a => a.ExtraContactInfo)).SetName("to denormalised record extra contact info mapping test");
            yield return new TestCaseData(new DateTime(2016, 12, 7, 17, 9, 44), Create(a => a.ReportMetadata.Range.Begin = 1481130584), (Func<DenormalisedRecord, DateTime>)(a => a.BeginDate)).SetName("to denormalised record begin date mapping test");
            yield return new TestCaseData(new DateTime(2016, 12, 7, 17, 9, 44), Create(a => a.ReportMetadata.Range.End = 1481130584), (Func<DenormalisedRecord, DateTime>)(a => a.EndDate)).SetName("to denormalised record end date mapping test");
            yield return new TestCaseData("Domain", Create(a => a.PolicyPublished.Domain = "Domain"), (Func<DenormalisedRecord, string>)(a => a.Domain)).SetName("to denormalised record domain mapping test");
            yield return new TestCaseData(Alignment.r, Create(a => a.PolicyPublished.Adkim = Alignment.r), (Func<DenormalisedRecord, Alignment>)(a => a.Adkim.Value)).SetName("to denormalised record adkim mapping test");
            yield return new TestCaseData(Alignment.r, Create(a => a.PolicyPublished.Aspf = Alignment.r), (Func<DenormalisedRecord, Alignment>)(a => a.Aspf.Value)).SetName("to denormalised record aspf mapping test");
            yield return new TestCaseData(Disposition.none, Create(a => a.PolicyPublished.P = Disposition.none), (Func<DenormalisedRecord, Disposition>)(a => a.P)).SetName("to denormalised record p mapping test");
            yield return new TestCaseData(Disposition.none, Create(a => a.PolicyPublished.Sp = Disposition.none), (Func<DenormalisedRecord, Disposition>)(a => a.Sp.Value)).SetName("to denormalised record sp mapping test");
            yield return new TestCaseData(100, Create(a => a.PolicyPublished.Pct = 100), (Func<DenormalisedRecord, int>)(a => a.Pct.Value)).SetName("to denormalised record pct mapping test");
            yield return new TestCaseData("envelopeTo", Create(a => a.Records[0].Identifiers.EnvelopeTo = "envelopeTo"), (Func<DenormalisedRecord, string>)(a => a.EnvelopeTo)).SetName("to denormalised record envelope to mapping test");
            yield return new TestCaseData("headerFrom", Create(a => a.Records[0].Identifiers.HeaderFrom = "headerFrom"), (Func<DenormalisedRecord, string>)(a => a.HeaderFrom)).SetName("to denormalised record header from mapping test");
            yield return new TestCaseData("10.0.0.0", Create(a => a.Records[0].Row.SourceIp = "10.0.0.0"), (Func<DenormalisedRecord, string>)(a => a.SourceIp)).SetName("to denormalised record source ip mapping test");
            yield return new TestCaseData(10, Create(a => a.Records[0].Row.Count = 10), (Func<DenormalisedRecord, int>)(a => a.Count)).SetName("to denormalised record count mapping test");
            yield return new TestCaseData(Disposition.none, Create(a => a.Records[0].Row.PolicyEvaluated.Disposition = Disposition.none), (Func<DenormalisedRecord, Disposition>)(a => a.Disposition.Value)).SetName("to denormalised record count mapping test");
            yield return new TestCaseData(DmarcResult.pass, Create(a => a.Records[0].Row.PolicyEvaluated.Dkim = DmarcResult.pass), (Func<DenormalisedRecord, DmarcResult>)(a => a.Dkim.Value)).SetName("to denormalised record dkim mapping test");
            yield return new TestCaseData(DmarcResult.pass, Create(a => a.Records[0].Row.PolicyEvaluated.Spf = DmarcResult.pass), (Func<DenormalisedRecord, DmarcResult>)(a => a.Spf.Value)).SetName("to denormalised record spf mapping test");

            yield return new TestCaseData("forwarded", Create( a =>
            {
                a.Records[0].Row.PolicyEvaluated.Reasons[0].PolicyOverride = PolicyOverride.forwarded;
            }), (Func<DenormalisedRecord, string>)(a => a.Reason)).SetName("to denormalised record policy override mapping test single entry");

            yield return new TestCaseData("forwarded,sampled_out", Create(a =>
            {
                a.Records[0].Row.PolicyEvaluated.Reasons[0].PolicyOverride = PolicyOverride.forwarded;
                a.Records[0].Row.PolicyEvaluated.Reasons[1].PolicyOverride = PolicyOverride.sampled_out;
            }, 2), (Func<DenormalisedRecord, string>)(a => a.Reason)).SetName("to denormalised record policy override mapping test multiple entries");

            yield return new TestCaseData("Comment 1", Create(a =>
            {
                a.Records[0].Row.PolicyEvaluated.Reasons[0].Comment = "Comment 1";
            }), (Func<DenormalisedRecord, string>)(a => a.Comment)).SetName("to denormalised record comment mapping test single entry");

            yield return new TestCaseData("Comment 1,Comment 2", Create(a =>
            {
                a.Records[0].Row.PolicyEvaluated.Reasons[0].Comment = "Comment 1";
                a.Records[0].Row.PolicyEvaluated.Reasons[1].Comment = "Comment 2";
            },2), (Func<DenormalisedRecord, string>)(a => a.Comment)).SetName("to denormalised record comment mapping test multiple entries");
            
            yield return new TestCaseData("domain", Create(a =>
            {
                a.Records[0].AuthResults.Dkim[0].Domain = "domain";
            }), (Func<DenormalisedRecord, string>)(a => a.DkimDomain)).SetName("to denormalised record dkim auth result domain mapping test single entry");

            yield return new TestCaseData("domain1,domain2", Create(a =>
            {
                a.Records[0].AuthResults.Dkim[0].Domain = "domain1";
                a.Records[0].AuthResults.Dkim[1].Domain = "domain2";
            }, dkimAuthResultCount: 2), (Func<DenormalisedRecord, string>)(a => a.DkimDomain)).SetName("to denormalised record dkim auth result domain mapping test multiple entries");

            yield return new TestCaseData("neutral", Create(a =>
            {
                a.Records[0].AuthResults.Dkim[0].Result = DkimResult.neutral;
            }), (Func<DenormalisedRecord, string>)(a => a.DkimResult)).SetName("to denormalised record dkim auth result result mapping test single entry");

            yield return new TestCaseData("neutral,pass", Create(a =>
            {
                a.Records[0].AuthResults.Dkim[0].Result = DkimResult.neutral;
                a.Records[0].AuthResults.Dkim[1].Result = DkimResult.pass;
            }, dkimAuthResultCount: 2), (Func<DenormalisedRecord, string>)(a => a.DkimResult)).SetName("to denormalised record dkim auth result result mapping test multiple entries");

            yield return new TestCaseData("hr1", Create(a =>
            {
                a.Records[0].AuthResults.Dkim[0].HumanResult = "hr1";
            }), (Func<DenormalisedRecord, string>)(a => a.DkimHumanResult)).SetName("to denormalised record dkim auth result human result mapping test single entry");

            yield return new TestCaseData("hr1,hr2", Create(a =>
            {
                a.Records[0].AuthResults.Dkim[0].HumanResult = "hr1";
                a.Records[0].AuthResults.Dkim[1].HumanResult = "hr2";
            }, dkimAuthResultCount: 2), (Func<DenormalisedRecord, string>)(a => a.DkimHumanResult)).SetName("to denormalised record dkim auth result human result mapping test multiple entries");

            yield return new TestCaseData("domain", Create(a =>
            {
                a.Records[0].AuthResults.Spf[0].Domain = "domain";
            }), (Func<DenormalisedRecord, string>)(a => a.SpfDomain)).SetName("to denormalised record spf auth result domain mapping test single entry");

            yield return new TestCaseData("domain1,domain2", Create(a =>
            {
                a.Records[0].AuthResults.Spf[0].Domain = "domain1";
                a.Records[0].AuthResults.Spf[1].Domain = "domain2";
            }, spfAuthResultCount: 2), (Func<DenormalisedRecord, string>)(a => a.SpfDomain)).SetName("to denormalised record spf auth result domain mapping test multiple entries");

            yield return new TestCaseData("softfail", Create(a =>
            {
                a.Records[0].AuthResults.Spf[0].Result = SpfResult.softfail;
            }), (Func<DenormalisedRecord, string>)(a => a.SpfResult)).SetName("to denormalised record spf auth result result mapping test single entry");

            yield return new TestCaseData("softfail,permerror", Create(a =>
            {
                a.Records[0].AuthResults.Spf[0].Result = SpfResult.softfail;
                a.Records[0].AuthResults.Spf[1].Result = SpfResult.permerror;
            }, spfAuthResultCount: 2), (Func<DenormalisedRecord, string>)(a => a.SpfResult)).SetName("to denormalised record spf auth result result mapping test multiple entries");
        }

        private static Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.AggregateReport Create(Action<Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.AggregateReport> setter, int policyOverrideCount = 1, int dkimAuthResultCount = 1, int spfAuthResultCount = 1)
        {
            PolicyOverrideReason[] policyOverrideReasons = Enumerable.Range(0, policyOverrideCount).Select(_ => new PolicyOverrideReason()).ToArray();
            DkimAuthResult[] dkimAuthResults = Enumerable.Range(0, dkimAuthResultCount).Select(_ => new DkimAuthResult()).ToArray();
            SpfAuthResult[] spfAuthResults = Enumerable.Range(0, spfAuthResultCount).Select(_ => new SpfAuthResult()).ToArray();

            var aggregateReport = new Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.AggregateReport
            {
                ReportMetadata = new ReportMetadata
                {
                    Range = new DateRange()
                },
                PolicyPublished = new PolicyPublished(),
                Records = new[]
                {
                    new Record
                    {
                        Identifiers = new Identifier(),
                        Row = new Row
                        {
                            PolicyEvaluated = new PolicyEvaluated
                            {
                                Reasons = policyOverrideReasons
                            }
                        },
                        AuthResults = new AuthResult
                        {
                            Dkim = dkimAuthResults,
                            Spf = spfAuthResults
                        }
                    }
                }
            };
            setter(aggregateReport);
            return aggregateReport;
        }
    }
}
