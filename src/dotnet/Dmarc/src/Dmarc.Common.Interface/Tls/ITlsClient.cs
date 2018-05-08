using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Tls.Domain;

namespace Dmarc.Common.Interface.Tls
{
    public interface ITlsClient
    {
        Task Connect(string host, int port);
        Task<TlsConnectionResult> Connect(string host, int port, TlsVersion version, List<CipherSuite> cipherSuites);
        NetworkStream GetStream();
        void Disconnect();
    }
}