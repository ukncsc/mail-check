using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Tls;
using Dmarc.Common.Tls.BouncyCastle;
using Dmarc.MxSecurityTester.Smtp;

namespace Dmarc.MxSecurityTester.Tls
{
    internal class SmtpTlsClient : TlsClient
    {
        private readonly ISmtpClient _smtpClient;
        private readonly ILogger _log;

        public SmtpTlsClient(ISmtpClient smtpClient,
            ITlsClientConfig tlsClientConfig,
            ILogger log)
            : base(log, tlsClientConfig)
        {
            _smtpClient = smtpClient;
            _log = log;
        }

        public override Task<StartTlsResult> TryInitializeSession(NetworkStream stream)
        {
            return _smtpClient.TryStartTls(stream);
        }
    }
}
