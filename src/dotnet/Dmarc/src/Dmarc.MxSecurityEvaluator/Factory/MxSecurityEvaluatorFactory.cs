using System;
using System.Collections.Generic;
using Amazon.SimpleSystemsManagement;
using Amazon.SQS;
using Amazon.SQS.Model;
using Dmarc.Common.Data;
using Dmarc.Common.Encryption;
using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Logging;
using Dmarc.Common.Messaging.Sqs.QueueProcessor;
using Dmarc.MxSecurityEvaluator.Config;
using Dmarc.MxSecurityEvaluator.Dao;
using Dmarc.MxSecurityEvaluator.Domain;
using Dmarc.MxSecurityEvaluator.Evaluators;
using Dmarc.MxSecurityEvaluator.Processors;
using Microsoft.Extensions.DependencyInjection;

namespace Dmarc.MxSecurityEvaluator.Factory
{
    internal static class MxSecurityEvaluatorFactory
    {
        public static ITlsRecordProcessor CreateQueueProcessor()
        {
            return BuildCommonServiceCollection()
                .AddTransient<ITlsRecordProcessor, TlsRecordProcessorQueue>()
                .AddTransient<IQueueProcessor<Message>, SqsLongPollingQueueProcessor>()
                .AddTransient<ISqsConfig, MxSecurityEvaluatorSqsConfig>()
                .AddTransient<IAmazonSQS, AmazonSQSClient>()
                .BuildServiceProvider()
                .GetService<ITlsRecordProcessor>();
        }

        public static ITlsRecordProcessor CreateManualProcessor(List<int> ids)
        {
            return BuildCommonServiceCollection()
                .AddTransient<ITlsRecordProcessor, TlsRecordProcessorManual>()
                .AddTransient<IMxSecurityEvaluatorArguments>(p => new MxSecurityEvaluatorArguments(ids))
                .BuildServiceProvider()
                .GetService<ITlsRecordProcessor>();
        }

        private static IServiceCollection BuildCommonServiceCollection()
        {
            return new ServiceCollection()
                .AddTransient<ILogger, ConsoleLogger>()
                .AddTransient<IConnectionInfo>(p =>
                    new StringConnectionInfo(Environment.GetEnvironmentVariable("ConnectionString")))
                .AddTransient<IConnectionInfoAsync, ConnectionInfoAsync>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<IParameterStoreRequest, ParameterStoreRequest>()
                .AddTransient<IAmazonSimpleSystemsManagement, AmazonSimpleSystemsManagementClient>()
                .AddTransient<ITlsRecordDao, TlsRecordDao>()
                .AddTransient<ITlsEvaluator, Ssl3FailsWithBadCipherSuite>()
                .AddTransient<ITlsEvaluator, Tls10AvailableWithBestCipherSuiteSelected>()
                .AddTransient<ITlsEvaluator, Tls10AvailableWithWeakCipherSuiteNotSelected>()
                .AddTransient<ITlsEvaluator, Tls11AvailableWithBestCipherSuiteSelected>()
                .AddTransient<ITlsEvaluator, Tls11AvailableWithWeakCipherSuiteNotSelected>()
                .AddTransient<ITlsEvaluator, Tls12AvailableWithBestCipherSuiteSelected>()
                .AddTransient<ITlsEvaluator, Tls12AvailableWithBestCipherSuiteSelectedFromReverseList>()
                .AddTransient<ITlsEvaluator, Tls12AvailableWithSha2HashFunctionSelected>()
                .AddTransient<ITlsEvaluator, Tls12AvailableWithWeakCipherSuiteNotSelected>()
                .AddTransient<ITlsEvaluator, TlsSecureDiffieHellmanGroupSelected>()
                .AddTransient<ITlsEvaluator, TlsSecureEllipticCurveSelected>()
                .AddTransient<ITlsEvaluator, TlsWeakCipherSuitesRejected>()
                .AddTransient<IMxSecurityEvaluator, MxSecurityEvaluator>();
        }
    }
}
