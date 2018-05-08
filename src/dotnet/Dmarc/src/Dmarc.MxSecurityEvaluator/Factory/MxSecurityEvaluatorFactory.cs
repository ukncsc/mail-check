using Amazon.SimpleSystemsManagement;
using Amazon.SQS;
using Amazon.SQS.Model;
using Dmarc.Common.Data;
using Dmarc.Common.Encryption;
using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Logging;
using Dmarc.Common.Messaging.Sqs.QueueProcessor;
using Dmarc.MxSecurityEvaluator.Config;
using Dmarc.MxSecurityEvaluator.Dao;
using Dmarc.MxSecurityEvaluator.Evaluators;
using Dmarc.MxSecurityEvaluator.Processors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Dmarc.Common.Interface.Logging;
using Dmarc.MxSecurityEvaluator.Domain;

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
                .AddTransient<ISsl3FailsWithBadCipherSuite, Ssl3FailsWithBadCipherSuite>()
                .AddTransient<ITls10AvailableWithBestCipherSuiteSelected, Tls10AvailableWithBestCipherSuiteSelected>()
                .AddTransient<ITls10AvailableWithWeakCipherSuiteNotSelected,
                    Tls10AvailableWithWeakCipherSuiteNotSelected>()
                .AddTransient<ITls11AvailableWithBestCipherSuiteSelected, Tls11AvailableWithBestCipherSuiteSelected>()
                .AddTransient<ITls11AvailableWithWeakCipherSuiteNotSelected,
                    Tls11AvailableWithWeakCipherSuiteNotSelected>()
                .AddTransient<ITls12AvailableWithBestCipherSuiteSelected, Tls12AvailableWithBestCipherSuiteSelected>()
                .AddTransient<ITls12AvailableWithBestCipherSuiteSelectedFromReverseList,
                    Tls12AvailableWithBestCipherSuiteSelectedFromReverseList>()
                .AddTransient<ITls11AvailableWithFallbackScsvSupport, Tls11AvailableWithFallbackScsvSupport>()
                .AddTransient<ITls12AvailableWithSha2HashFunctionSelected, Tls12AvailableWithSha2HashFunctionSelected>()
                .AddTransient<ITls12AvailableWithWeakCipherSuiteNotSelected,
                    Tls12AvailableWithWeakCipherSuiteNotSelected>()
                .AddTransient<ITlsSecureDiffieHellmanGroupSelected, TlsSecureDiffieHellmanGroupSelected>()
                .AddTransient<ITlsSecureEllipticCurveSelected, TlsSecureEllipticCurveSelected>()
                .AddTransient<ITlsWeakCipherSuitesRejected, TlsWeakCipherSuitesRejected>()
                .AddTransient<IMxSecurityEvaluator, MxSecurityEvaluator>();
        }
    }
}
