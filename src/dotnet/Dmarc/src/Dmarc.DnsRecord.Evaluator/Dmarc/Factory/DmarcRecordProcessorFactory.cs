using Amazon.SimpleSystemsManagement;
using Amazon.SQS;
using Amazon.SQS.Model;
using Dmarc.Common.Data;
using Dmarc.Common.Encryption;
using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Logging;
using Dmarc.Common.Messaging.Sqs.QueueProcessor;
using Dmarc.Common.PublicSuffix;
using Dmarc.DnsRecord.Evaluator.Config;
using Dmarc.DnsRecord.Evaluator.Dmarc.Dao;
using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Dmarc.Explainers;
using Dmarc.DnsRecord.Evaluator.Dmarc.Implict;
using Dmarc.DnsRecord.Evaluator.Dmarc.Parsers;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Config;
using Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record;
using Dmarc.DnsRecord.Evaluator.Explainers;
using Dmarc.DnsRecord.Evaluator.Implicit;
using Dmarc.DnsRecord.Evaluator.Rules;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Factory
{
    public class DmarcRecordProcessorFactory
    {
        public static IDmarcRecordProcessor Create()
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                .AddTransient<IDmarcConfigParser, DmarcConfigParser>()
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
                .AddTransient<IRuleEvaluator<DmarcConfig>, RuleEvaluator<DmarcConfig>>()
                .AddTransient<IRule<DmarcConfig>, OnlyOneDmarcRecord>()
                .AddTransient<IRule<DmarcConfig>, TldDmarcRecordBehaviourIsWeaklyDefined>()
                .AddTransient<IRuleEvaluator<DmarcRecord>, RuleEvaluator<DmarcRecord>>()
                .AddTransient<IRule<DmarcRecord>, VersionMustBeFirstTag>()
                .AddTransient<IRule<DmarcRecord>, MaxLengthOf450Characters>()
                .AddTransient<IRule<DmarcRecord>, PctValueShouldBe100>()
                .AddTransient<IRule<DmarcRecord>, PolicyShouldBeQuarantineOrReject>()
                .AddTransient<IRule<DmarcRecord>, PolicyTagMustExist>()
                .AddTransient<IRule<DmarcRecord>, RuaTagShouldHaveUris>()
                .AddTransient<IRule<DmarcRecord>, RuaTagShouldNotHaveMoreThanTwoUris>()
                .AddTransient<IRule<DmarcRecord>, RuaTagsShouldBeMailTo>()
                .AddTransient<IRule<DmarcRecord>, RuaTagsShouldContainDmarcServiceMailBox>()
                .AddTransient<IRule<DmarcRecord>, RufTagShouldBeMailTo>()
                .AddTransient<IRule<DmarcRecord>, RufTagShouldNotHaveMoreThanTwoUris>()
                .AddTransient<IRule<DmarcRecord>, SubDomainPolicyShouldBeQuarantineOrReject>()
                .AddTransient<IRule<DmarcRecord>, SubDomainPolicyShouldNotBeOnNonOrganisationalDomain>()
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
                .AddTransient<IDmarcConfigReadModelDao, DmarcConfigReadModelDao>()
                .AddTransient<IDmarcRecordProcessor, DmarcRecordProcessor>()
                .AddTransient<IQueueProcessor<Message>, SqsLongPollingQueueProcessor>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<ISqsConfig, RecordEvaluatorConfig>()
                .AddTransient<ILogger, ConsoleLogger>()
                .AddTransient<IAmazonSQS, AmazonSQSClient>()
                .AddTransient<IAmazonSimpleSystemsManagement, AmazonSimpleSystemsManagementClient>()
                .AddTransient<IParameterStoreRequest, ParameterStoreRequest>()
                .AddTransient<IConnectionInfoAsync, ConnectionInfoAsync>()
                .AddTransient<IConnectionInfo>(_ => new StringConnectionInfo(Environment.GetEnvironmentVariable("ConnectionString")))
                .AddSingleton<IOrganisationalDomainProvider, OrganisationDomainProvider>()
                .BuildServiceProvider();

            return serviceProvider.GetService<IDmarcRecordProcessor>();
        }
    }
}
