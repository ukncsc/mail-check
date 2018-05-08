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

            bool sessionInitialized = await TryInitializeSession(_tcpClient.GetStream()).ConfigureAwait(false);

            if (!sessionInitialized)
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
                _log.Error($"An error occurred {e.Message}{System.Environment.NewLine}{e.StackTrace}");
                return new TlsConnectionResult(Error.TCP_CONNECTION_FAILED);
            }
            catch (ArgumentNullException e)
            {
                _log.Error($"An error occurred {e.Message}{System.Environment.NewLine}{e.StackTrace}");
                return new TlsConnectionResult(Error.TCP_CONNECTION_FAILED);
            }
            catch (IOException e)
            {
                _log.Error($"An error occurred {e.Message}{System.Environment.NewLine}{e.StackTrace}");
                return new TlsConnectionResult(Error.TCP_CONNECTION_FAILED);
            }
            catch (TimeoutException e)
            {
                _log.Error($"An error occurred {e.Message}{System.Environment.NewLine}{e.StackTrace}");
                return new TlsConnectionResult(Error.TCP_CONNECTION_FAILED);
            }
            catch (Exception e)
            {
                _log.Error($"An error occurred {e.Message}{System.Environment.NewLine}{e.StackTrace}");
                return new TlsConnectionResult(Error.INTERNAL_ERROR);
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
            _log.Debug($"Synchronization context: {SynchronizationContext.Current?.ToString() ?? "null"}");
            await _tcpClient.ConnectAsync(host, port).ConfigureAwait(false);
            _log.Debug($"Successfully started TCP connection to {host ?? "<null>"}:{port}");

            _log.Debug("Initializing session");
            bool sessionInitialized = await TryInitializeSession(_tcpClient.GetStream()).ConfigureAwait(false);
            _log.Debug("Successfully initialized session");

            if (!sessionInitialized)
            {
                _log.Debug("Failed to initialize session");
                return new TlsConnectionResult(Error.SESSION_INITIALIZATION_FAILED);
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
        public virtual Task<bool> TryInitializeSession(NetworkStream stream)
        {
            return Task.FromResult(true);
        }

        public void Disconnect()
        {
            _tcpClient?.Dispose();
        }
    }
}
