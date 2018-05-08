using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Converters;
using Dmarc.Common.Report.Domain;
using NUnit.Framework;
using Dmarc.AggregateReport.Parser.Lambda.Dao.Entities;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc;
using AggregateReportDomain =  Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.AggregateReport;
using AlignmentDomain = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.Alignment;
using DispositionDomain = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.Disposition;
using DmarcResultDomain = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.DmarcResult;
using PolicyOverrideDomain = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.PolicyOverride;
using SpfResultDomain = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.SpfResult;
using DkimResultDomain = Dmarc.AggregateReport.Parser.Lambda.Domain.Dmarc.DkimResult;


namespace Dmarc.Lambda.AggregateReport.Parser.Test.Converters
{
    [TestFixture]
    internal class AggregateReportToEntityConverterTests
    {
        private AggregateReportToEntityConverter _aggregateReportToEntityConverter;

        [SetUp]
        public void SetUp()
        {
            _aggregateReportToEntityConverter = new AggregateReportToEntityConverter();
        }

        [Test]
        [TestCaseSource(nameof(CreateConversionTestData))]
        public void MappingTests<TOut>(TOut expected, AggregateReportInfo aggregateReportInfo, Func<AggregateReportEntity, TOut> actualGetter)
        {
            AggregateReportEntity aggregateReportEntity = _aggregateReportToEntityConverter.Convert(aggregateReportInfo);
            Assert.That(expected, Is.EqualTo(actualGetter(aggregateReportEntity)));
        }


        public static IEnumerable<TestCaseData> CreateConversionTestData()
        {
            yield return new TestCaseData("0123456789", Create(requestId: "0123456789"), (Func<AggregateReportEntity, string>)(a => a.RequestId)).SetName("convert to entity request id mapping test");
            yield return new TestCaseData("bucket/0123456789", Create(orginalUri: "bucket/0123456789"), (Func<AggregateReportEntity, string>)(a => a.OrginalUri)).SetName("convert to entity original uri mapping test");
            yield return new TestCaseData("0123456789.gz", Create(attachmentFilename: "0123456789.gz"), (Func<AggregateReportEntity, string>)(a => a.AttachmentFilename)).SetName("convert to entity attachment filename mapping test");
            yield return new TestCaseData("0123456789", Create(ard => ard.ReportMetadata.ReportId = "0123456789"), (Func<AggregateReportEntity, string>)(a => a.ReportId)).SetName("convert to entity report id mapping test");
            yield return new TestCaseData("dmarc@provider.com", Create(ard => ard.ReportMetadata.Email = "dmarc@provider.com"), (Func<AggregateReportEntity, string>)(a => a.Email)).SetName("convert to entity email mapping test");
            yield return new TestCaseData("provider", Create(ard => ard.ReportMetadata.OrgName = "provider"), (Func<AggregateReportEntity, string>)(a => a.OrgName)).SetName("convert to entity org name mapping test");
            yield return new TestCaseData("dmarc_query@provider.com", Create(ard => ard.ReportMetadata.ExtraContactInfo = "dmarc_query@provider.com"), (Func<AggregateReportEntity, string>)(a => a.ExtraContactInfo)).SetName("convert to extra contact info mapping test");
            yield return new TestCaseData(new DateTime(2016, 12, 7, 17, 9, 44), Create(ard => ard.ReportMetadata.Range.Begin = 1481130584), (Func<AggregateReportEntity, DateTime>)(a => a.BeginDate)).SetName("convert to entity begin date mapping test");
            yield return new TestCaseData(new DateTime(2016, 12, 7, 17, 9, 44), Create(ard => ard.ReportMetadata.Range.End = 1481130584), (Func<AggregateReportEntity, DateTime>)(a => a.EndDate)).SetName("convert to entity end date mapping test");
            yield return new TestCaseData("gov.uk", Create(ard => ard.PolicyPublished.Domain = "gov.uk"), (Func<AggregateReportEntity, string>)(a => a.Domain)).SetName("convert to entity domain mapping test");
            yield return new TestCaseData(EntityAlignment.r, Create(ard => ard.PolicyPublished.Adkim = AlignmentDomain.r), (Func<AggregateReportEntity, EntityAlignment?>)(a => a.Adkim)).SetName("convert to entity adkim mapping test");
            yield return new TestCaseData(EntityAlignment.s, Create(ard => ard.PolicyPublished.Aspf = AlignmentDomain.s), (Func<AggregateReportEntity, EntityAlignment?>)(a => a.Aspf)).SetName("convert to entity aspf mapping test");
            yield return new TestCaseData(EntityDisposition.quarantine, Create(ard => ard.PolicyPublished.P = DispositionDomain.quarantine), (Func<AggregateReportEntity, EntityDisposition>)(a => a.P)).SetName("convert to entity p mapping test");
            yield return new TestCaseData(EntityDisposition.reject, Create(ard => ard.PolicyPublished.Sp = DispositionDomain.reject), (Func<AggregateReportEntity, EntityDisposition?>)(a => a.Sp)).SetName("convert to entity sp mapping test");
            yield return new TestCaseData(100, Create(ard => ard.PolicyPublished.Pct = 100), (Func<AggregateReportEntity, int?>)(a => a.Pct)).SetName("convert to entity pct mapping test");

            yield return new TestCaseData(1, Create(ard => ard.Records.First().Row.Count = 1), (Func<AggregateReportEntity, int?>)(a => a.Records.First().Count)).SetName("convert to entity count mapping test");
            yield return new TestCaseData("0.0.0.0", Create(ard => ard.Records.First().Row.SourceIp = "0.0.0.0"), (Func<AggregateReportEntity, string>)(a => a.Records.First().SourceIp)).SetName("convert to entity ip mapping test");
            yield return new TestCaseData(EntityDisposition.reject, Create(ard => ard.Records.First().Row.PolicyEvaluated.Disposition = DispositionDomain.reject), (Func<AggregateReportEntity, EntityDisposition?>)(a => a.Records.First().Disposition)).SetName("convert to entity disposition mapping test");
            yield return new TestCaseData(EntityDmarcResult.pass, Create(ard => ard.Records.First().Row.PolicyEvaluated.Dkim = DmarcResultDomain.pass), (Func<AggregateReportEntity, EntityDmarcResult?>)(a => a.Records.First().Dkim)).SetName("convert to entity dmarc result mapping test");
            yield return new TestCaseData(EntityDmarcResult.pass, Create(ard => ard.Records.First().Row.PolicyEvaluated.Spf = DmarcResultDomain.pass), (Func<AggregateReportEntity, EntityDmarcResult?>)(a => a.Records.First().Spf)).SetName("convert to entity spf result mapping test");
            yield return new TestCaseData("Comment", Create(ard => ard.Records.First().Row.PolicyEvaluated.Reasons.First().Comment = "Comment"), (Func<AggregateReportEntity, string>)(a => a.Records.First().Reason.First().Comment)).SetName("convert to entity comment mapping test");
            yield return new TestCaseData(EntityPolicyOverride.forwarded, Create(ard => ard.Records.First().Row.PolicyEvaluated.Reasons.First().PolicyOverride = PolicyOverrideDomain.forwarded), (Func<AggregateReportEntity, EntityPolicyOverride?>)(a => a.Records.First().Reason.First().PolicyOverride)).SetName("convert to entity policy override mapping test");

            yield return new TestCaseData("envelopeto", Create(ard => ard.Records.First().Identifiers.EnvelopeTo = "envelopeto"), (Func<AggregateReportEntity, string>)(a => a.Records.First().EnvelopeTo)).SetName("convert to entity envelope to mapping test");
            yield return new TestCaseData("headerfrom", Create(ard => ard.Records.First().Identifiers.HeaderFrom = "headerfrom"), (Func<AggregateReportEntity, string>)(a => a.Records.First().HeaderFrom)).SetName("convert to entity header from to mapping test");

            yield return new TestCaseData("gov.uk", Create(ard => ard.Records.First().AuthResults.Spf.First().Domain = "gov.uk"), (Func<AggregateReportEntity, string>)(a => a.Records.First().SpfAuthResults.First().Domain)).SetName("convert to entity spf auth result domain mapping test");
            yield return new TestCaseData(EntitySpfResult.neutral, Create(ard => ard.Records.First().AuthResults.Spf.First().Result = SpfResultDomain.neutral), (Func<AggregateReportEntity, EntitySpfResult?>)(a => a.Records.First().SpfAuthResults.First().Result)).SetName("convert to entity spf auth result mapping test");

            yield return new TestCaseData("gov.uk", Create(ard => ard.Records.First().AuthResults.Dkim.First().Domain = "gov.uk"), (Func<AggregateReportEntity, string>)(a => a.Records.First().DkimAuthResults.First().Domain)).SetName("convert to entity dkim auth result domain mapping test");
            yield return new TestCaseData(EntityDkimResult.neutral, Create(ard => ard.Records.First().AuthResults.Dkim.First().Result = DkimResultDomain.neutral), (Func<AggregateReportEntity, EntityDkimResult?>)(a => a.Records.First().DkimAuthResults.First().Result)).SetName("convert to entity dkim auth result mapping test");
            yield return new TestCaseData("human result", Create(ard => ard.Records.First().AuthResults.Dkim.First().HumanResult = "human result"), (Func<AggregateReportEntity,string>)(a => a.Records.First().DkimAuthResults.First().HumanResult)).SetName("convert to entity dkim auth human result mapping test");
        }

        private static AggregateReportInfo Create(string requestId = "RequestId", string orginalUri = "OriginalUri",
            string emailFilename = "EmailFilename", long filesize = 1, string attachmentFilename = "AttachmentFilename")
        {
            AggregateReportDomain aggregateReport = new AggregateReportDomain
            {
                ReportMetadata = new ReportMetadata
                {
                    Range = new DateRange()
                },
                PolicyPublished = new PolicyPublished(),
                Records = new[] { new Record
                    {
                        Row = new Row
                        {
                            PolicyEvaluated = new PolicyEvaluated
                            {
                                Reasons = new []
                                {
                                    new PolicyOverrideReason(), 
                                }
                            }
                        },
                        Identifiers = new Identifier(),
                        AuthResults = new AuthResult
                        {
                            Dkim = new []
                            {
                                new DkimAuthResult()
                            },
                            Spf = new []
                            {
                                new SpfAuthResult()
                            }
                        }
                    }
                }
            };
            EmailMetadata emailMetadata = new EmailMetadata(requestId, string.Empty, orginalUri, emailFilename, filesize);
            AttachmentMetadata attachmentMetadata = new AttachmentMetadata(attachmentFilename);

            return new AggregateReportInfo(aggregateReport, emailMetadata, attachmentMetadata);
        }

        private static AggregateReportInfo Create(Action<AggregateReportDomain> setter)
        {
            AggregateReportInfo aggregateReportInfo = Create();
            setter(aggregateReportInfo.AggregateReport);
            return aggregateReportInfo;
        }
    }
}
