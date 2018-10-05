using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Tls;
using Dmarc.Common.Interface.Tls.Domain;
using Dmarc.Common.Logging;
using Dmarc.Common.Tasks;
using Org.BouncyCastle.Crypto.Tls;
using Org.BouncyCastle.Security;
using CipherSuite = Dmarc.Common.Interface.Tls.Domain.CipherSuite;

namespace Dmarc.Common.Tls.BouncyCastle
{
    public class StartTlsResult
    {
        public StartTlsResult(bool success, List<string> smtpSession, string error)
        {
            Success = success;
            SmtpSession = smtpSession ?? new List<string>();
            Error = error;
        }

        public bool Success { get; }
        public List<string> SmtpSession { get; }
        public string Error { get; }
    }

    public class TlsClient : ITlsClient
    {
        private readonly TimeSpan _timeOut;

        private readonly ILogger _log;
        private TcpClient _tcpClient;

        public TlsClient(ILogger log,
            ITlsClientConfig config)
        {
            _log = log;
            _timeOut = config.TlsConnectionTimeOut;
        }

        public TlsClient()
        {
            _log = new ConsoleLogger();
            _timeOut = TimeSpan.FromSeconds(20);
        }

        public async Task Connect(string host, int port)
        {
            _tcpClient = new TcpClient();

            await _tcpClient.ConnectAsync(host, port).ConfigureAwait(false);

            StartTlsResult sessionInitialized = await TryInitializeSession(_tcpClient.GetStream()).ConfigureAwait(false);

            if (!sessionInitialized.Success)
            {
                throw new Exception("Failed to initialize session.");
            }

            TlsClientProtocol clientProtocol = new TlsClientProtocol(_tcpClient.GetStream(), SecureRandom.GetInstance("SHA256PRNG"));

            clientProtocol.Connect(new BasicTlsClient());
        }

        public async Task<TlsConnectionResult> Connect(string host, int port, TlsVersion version, List<CipherSuite> cipherSuites)
        {
            try
            {
                return await DoConnect(host, port, version, cipherSuites).TimeoutAfter(_timeOut).ConfigureAwait(false);
            }
            catch (SocketException e)
            {
                _log.Error($"{e.GetType().Name} occurred {e.Message}{System.Environment.NewLine}{e.StackTrace}");

                return e.SocketErrorCode == SocketError.HostNotFound
                    ? new TlsConnectionResult(Error.HOST_NOT_FOUND, e.Message, null)
                    : new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, e.Message, null);
            }
            catch (ArgumentNullException e)
            {
                _log.Error($"{e.GetType().Name} occurred {e.Message}{System.Environment.NewLine}{e.StackTrace}");
                return new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, e.Message, null);
            }
            catch (IOException e)
            {
                _log.Error($"{e.GetType().Name} occurred {e.Message}{System.Environment.NewLine}{e.StackTrace}");
                return new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, e.Message, null);
            }
            catch (TimeoutException e)
            {
                _log.Error($"{e.GetType().Name} occurred {e.Message}{System.Environment.NewLine}{e.StackTrace}");
                return new TlsConnectionResult(Error.TCP_CONNECTION_FAILED, e.Message, null);
            }
            catch (Exception e)
            {
                _log.Error($"{e.GetType().Name} occurred {e.Message}{System.Environment.NewLine}{e.StackTrace}");
                return new TlsConnectionResult(Error.INTERNAL_ERROR, e.Message, null);
            }
        }

        private async Task<TlsConnectionResult> DoConnect(string host, int port, TlsVersion version, List<CipherSuite> cipherSuites)
        {
            _tcpClient = new TcpClient
            {
                NoDelay = true,
                SendTimeout = _timeOut.Milliseconds,
                ReceiveTimeout = _timeOut.Milliseconds,
            };

            _log.Debug($"Starting TCP connection to {host ?? "<null>"}:{port}");
            await _tcpClient.ConnectAsync(host, port).ConfigureAwait(false);
            _log.Debug($"Successfully started TCP connection to {host ?? "<null>"}:{port}");

            _log.Debug("Initializing session");
            StartTlsResult sessionInitialized = await TryInitializeSession(_tcpClient.GetStream()).ConfigureAwait(false);
            _log.Debug("Successfully initialized session");

            if (!sessionInitialized.Success)
            {
                _log.Debug("Failed to initialize session");
                return new TlsConnectionResult(Error.SESSION_INITIALIZATION_FAILED, sessionInitialized.Error, sessionInitialized.SmtpSession);
            }

            TestTlsClientProtocol clientProtocol = new TestTlsClientProtocol(_tcpClient.GetStream());

            TestTlsClient testSuiteTlsClient = new TestTlsClient(version, cipherSuites);

            _log.Debug("Starting TLS session");
            TlsConnectionResult connectionResult = clientProtocol.ConnectWithResults(testSuiteTlsClient);
            _log.Debug("Successfully started TLS session");

            return connectionResult;
        }

        public NetworkStream GetStream()
        {
            return _tcpClient?.GetStream();
        }

        //Override this if for example you are using SMTP and you need to STARTTLS
        //before beginning a TLS session.
        public virtual Task<StartTlsResult> TryInitializeSession(NetworkStream stream)
        {
            return Task.FromResult(new StartTlsResult(false, null, string.Empty));
        }

        public void Disconnect()
        {
            _tcpClient?.Dispose();
        }
    }
}
