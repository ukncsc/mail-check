using System;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Tls;
using Dmarc.Common.Logging;
using Dmarc.MxSecurityTester.Config;
using Dmarc.MxSecurityTester.Console;
using Dmarc.MxSecurityTester.MxTester;
using Dmarc.MxSecurityTester.Smtp;
using Dmarc.MxSecurityTester.Tls;
using Dmarc.MxSecurityTester.Tls.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace Dmarc.MxSecurityTester.Factory
{
    internal static class MxSecurityTesterAppFactory
    {
        internal static IMxSecurityTesterDebugApp CreateMxSecurityTesterDebugApp()
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                .AddTransient<IMxSecurityTesterDebugApp, MxSecurityTesterDebugApp>()
                .AddTransient<ILogger, ConsoleLogger>()
                .AddTransient<ISmtpClient, SmtpClient>()
                .AddTransient<ITlsClient, SmtpTlsClient>()
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
                .AddTransient<ITlsSecurityTester, TlsSecurityTester>()
                .AddTransient<ISmtpSerializer, SmtpSerializer>()
                .AddTransient<ISmtpDeserializer, SmtpDeserializer>()
                .AddTransient<IMxSecurityTesterConfig, MxSecurityTesterAppConfig>()
                .AddTransient<ITlsClientConfig, MxSecurityTesterAppConfig>()
                .BuildServiceProvider();

            return serviceProvider.GetService<IMxSecurityTesterDebugApp>();
        }
    }
}
