using System;
using Amazon.SimpleSystemsManagement;
using Dmarc.DnsRecord.Evaluator.Config;
using Dmarc.DnsRecord.Evaluator.Spf.Explainers;
using Dmarc.DnsRecord.Evaluator.Spf.Parsers;
using Dmarc.DnsRecord.Evaluator.Spf.Rules.Config;
using Dmarc.DnsRecord.Evaluator.Spf.Rules.Record;
using Microsoft.Extensions.DependencyInjection;
using Amazon.SQS;
using Dmarc.Common.Data;
using Dmarc.Common.Encryption;
using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.DnsRecord.Evaluator.Explainers;
using Dmarc.DnsRecord.Evaluator.Implicit;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Messaging.Sqs.QueueProcessor;
using Dmarc.DnsRecord.Evaluator.Rules;
using Dmarc.DnsRecord.Evaluator.Spf.Dao;
using Dmarc.DnsRecord.Evaluator.Spf.Domain;
using Dmarc.DnsRecord.Evaluator.Spf.Implicit;
using Amazon.SQS.Model;

namespace Dmarc.DnsRecord.Evaluator.Spf.Factory
{
    public static class SpfRecordProcessorFactory
    {
        public static ISpfRecordProcessor Create()
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                .AddTransient<ISpfRecordProcessor, SpfRecordProcessor>()
                .AddTransient<IQueueProcessor<Message>, SqsLongPollingQueueProcessor>()
                .AddTransient<ISqsConfig, RecordEvaluatorConfig>()
                .AddTransient<IAmazonSQS, AmazonSQSClient>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<ILogger, ConsoleLogger>()
                .AddTransient<ISpfConfigParser, SpfConfigParser>()
                .AddTransient<ISpfRecordParser, SpfRecordParser>()
                .AddTransient<ISpfVersionParser, SpfVersionParser>()
                .AddTransient<ISpfConfigReadModelDao, SpfConfigReadModelDao>()
                .AddTransient<IConnectionInfoAsync, ConnectionInfoAsync>()
                .AddTransient<IParameterStoreRequest, ParameterStoreRequest>()
                .AddTransient<IAmazonSimpleSystemsManagement, AmazonSimpleSystemsManagementClient>()
                .AddTransient<IConnectionInfo>(_ => new StringConnectionInfo(Environment.GetEnvironmentVariable("ConnectionString")))
                .AddTransient<ITermParser, TermParser>()
                .AddTransient<IMechanismParser, MechanismParser>()
                .AddTransient<IQualifierParser, QualifierParser>()
                .AddTransient<IMechanismParserStrategy, AllMechanismParser>()
                .AddTransient<IMechanismParserStrategy, IncludeMechanismParser>()
                .AddTransient<IMechanismParserStrategy, AMechanismParser>()
                .AddTransient<IMechanismParserStrategy, MxMechanismParser>()
                .AddTransient<IMechanismParserStrategy, PtrMechanismParser>()
                .AddTransient<IMechanismParserStrategy, Ip4MechanismParser>()
                .AddTransient<IMechanismParserStrategy, Ip6MechanismParser>()
                .AddTransient<IMechanismParserStrategy, ExistsMechanismParser>()
                .AddTransient<IDomainSpecDualCidrBlockMechanismParser, DomainSpecDualCidrBlockMechanismParser>()
                .AddTransient<IModifierParser, ModifierParser>()
                .AddTransient<IModifierParserStrategy, RedirectModifierParser>()
                .AddTransient<IModifierParserStrategy, ExplanationModifierParser>()
                .AddTransient<IDomainSpecParser, DomainSpecParserPassive>()
                .AddTransient<IDualCidrBlockParser, DualCidrBlockParser>()
                .AddTransient<IIp4CidrBlockParser, Ip4CidrBlockParser>()
                .AddTransient<IIp6CidrBlockParser, Ip6CidrBlockParser>()
                .AddTransient<IIp4AddrParser, Ip4AddrParser>()
                .AddTransient<IIp6AddrParser, Ip6AddrParser>()
                .AddTransient<IQualifierExplainer, QualifierExplainer>()
                .AddTransient<IExplainer<Domain.Version>, VersionExplainer>()
                .AddTransient<IExplainer<Term>, Explainer<Term>>()
                .AddTransient<IExplainerStrategy<Term>, AllTermExplainer>()
                .AddTransient<IExplainerStrategy<Term>, IncludeTermExplainer>()
                .AddTransient<IExplainerStrategy<Term>, ATermExplainer>()
                .AddTransient<IExplainerStrategy<Term>, MxTermExplainer>()
                .AddTransient<IExplainerStrategy<Term>, PtrTermExplainer>()
                .AddTransient<IExplainerStrategy<Term>, Ip4TermExplainer>()
                .AddTransient<IExplainerStrategy<Term>, Ip6TermExplainer>()
                .AddTransient<IExplainerStrategy<Term>, ExistsTermExplainer>()
                .AddTransient<IExplainerStrategy<Term>, RedirectTermExplainer>()
                .AddTransient<IExplainerStrategy<Term>, ExplanationTermExplainer>()
                .AddTransient<IExplainerStrategy<Term>, UnknownTermExplainer>()
                .AddTransient<IExplainerStrategy<Term>, UnknownModifierExplainer>()
                .AddTransient<IRule<SpfConfig>, OnlyOneSpfRecord>()
                .AddTransient<IRuleEvaluator<SpfConfig>, RuleEvaluator<SpfConfig>>()
                .AddTransient<IRuleEvaluator<SpfRecord>, RuleEvaluator<SpfRecord>>()
                .AddTransient<IRule<SpfRecord>, AllMustBeLastMechanism>()
                .AddTransient<IRule<SpfRecord>, DontUsePtrMechanism>()
                .AddTransient<IRule<SpfRecord>, ExplanationDoesntOccurMoreThanOnce>()
                .AddTransient<IRule<SpfRecord>, RedirectDoesntOccurMoreThanOnce>()
                .AddTransient<IRule<SpfRecord>, MaxLengthOf450Characters>()
                .AddTransient<IRule<SpfRecord>, ModifiersOccurAfterMechanisms>()
                .AddTransient<IRule<SpfRecord>, RedirectModifierAndAllMechanismNotValid>()
                .AddTransient<IRule<SpfRecord>, ShouldHaveHardFailAllEnabled>()
                .AddTransient<IImplicitProvider<Term>, ImplicitProvider<Term>>()
                .AddTransient<IImplicitProviderStrategy<Term>, AllImplicitTermProvider>()
                .BuildServiceProvider();

            return serviceProvider.GetService<ISpfRecordProcessor>();
        }
    }
}
