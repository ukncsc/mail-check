using System;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using Dmarc.Common.Data;
using Dmarc.Common.Encryption;
using Dmarc.Common.Environment;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Interface.Tls;
using Dmarc.Common.Logging;
using Dmarc.Common.Messaging.Sns.Publisher;
using Dmarc.Common.Util;
using Dmarc.MxSecurityTester.Caching;
using Dmarc.MxSecurityTester.Config;
using Dmarc.MxSecurityTester.Console;
using Dmarc.MxSecurityTester.Dao;
using Dmarc.MxSecurityTester.MxTester;
using Dmarc.MxSecurityTester.Scheduling;
using Dmarc.MxSecurityTester.Smtp;
using Dmarc.MxSecurityTester.Tls;
using Dmarc.MxSecurityTester.Tls.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace Dmarc.MxSecurityTester.Factory
{
    public static class MxSecurityTesterFactory
    {
        internal static IMxSecurityTesterProcessorRunner CreateMxSecurityTesterProcessorRunner()
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                .AddTransient<IMxSecurityTesterDebugApp, MxSecurityTesterDebugApp>()
                .AddTransient<ILogger, ConsoleLogger>()
                .AddTransient<ISmtpClient, SmtpClient>()
                .AddTransient<ITlsClient, SmtpTlsClient>()
                .AddTransient<ITlsSecurityTester, TlsSecurityTester>()
                .AddTransient<ITlsTest, Tls12AvailableWithBestCipherSuiteSelected>()
                .AddTransient<ITlsTest, Tls12AvailableWithBestCipherSuiteSelectedFromReversedList>()
                .AddTransient<ITlsTest, Tls12AvailableWithSha2HashFunctionSelected>()
                .AddTransient<ITlsTest, Tls12AvailableWithWeakCipherSuiteNotSelected>()
                .AddTransient<ITlsTest, Tls11AvailableWithBestCipherSuiteSelected>()
                .AddTransient<ITlsTest, Tls11AvailableWithWeakCipherSuiteNotSelected>()
                .AddTransient<ITlsTest, Tls10AvailableWithBestCipherSuiteSelected>()
                .AddTransient<ITlsTest, Tls10AvailableWithWeakCipherSuiteNotSelected>()
                .AddTransient<ITlsTest, Ssl3FailsWithBadCipherSuite>()
                .AddTransient<ITlsTest, TlsSecureEllipticCurveSelected>()
                .AddTransient<ITlsTest, TlsSecureDiffieHelmanGroupSelected>()
                .AddTransient<ITlsTest, TlsWeakCipherSuitesRejected>()
                .AddTransient<IPersistentTlsSecurityProfileUpdater, PersistentTlsSecurityProfileUpdater>()
                .AddTransient<IPublishingTlsSecurityProfileUpdater, PublishingTlsSecurityProfileUpdater>()
                .AddTransient<IPublishingCertsSecurityProfileUpdater, PublishingCertsSecurityProfileUpdater>()
                .AddTransient<IPublisher, SnsPublisher>()
                .AddTransient<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>()
                .AddTransient<IPublisherConfig, MxSecurityTesterConfig>()
                .AddTransient<ICertificatePublisherConfig, CertificatePublisherConfig>()
                .AddTransient<ITlsClientConfig, MxSecurityTesterConfig>()
                .AddTransient<IScheduler, Scheduler>()
                .AddSingleton<IMxSecurityTesterProcessorRunner, MxSecurityTesterProcessorRunner>()
                .AddTransient<IAmazonSimpleSystemsManagement, AmazonSimpleSystemsManagementClient>()
                .AddTransient<IParameterStoreRequest, ParameterStoreRequest>()
                .AddTransient<IConnectionInfo>(p => new StringConnectionInfo(Environment.GetEnvironmentVariable("ConnectionString")))
                .AddSingleton<IConnectionInfoAsync, ConnectionInfoAsync>()
                .AddTransient<IDomainTlsSecurityProfileDao, DomainTlsSecurityProfileDao>()
                .AddTransient<IMxSecurityTesterConfig, MxSecurityTesterConfig>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<IMxSecurityTesterProcessor, MxSecurityTesterProcessor>()
                .AddTransient<ITlsSecurityProfileUpdater, TlsSecurityProfileUpdater>()
                .AddTransient<ISmtpSerializer, SmtpSerializer>()
                .AddTransient<ISmtpDeserializer, SmtpDeserializer>()
                .AddTransient<ITlsSecurityTesterAdapator, TlsSecurityTesterAdapator>()
                .AddTransient<ICachingTlsSecurityTesterAdapator, CachingTlsSecurityTesterAdapator>()
                .AddTransient<ICache, RedisCache>()
                .AddTransient<IRedisConfig, MxSecurityTesterConfig>()
                .AddTransient<IClock, Clock>()
                .BuildServiceProvider();

            return serviceProvider.GetService<IMxSecurityTesterProcessorRunner>();
        }
    }
}
