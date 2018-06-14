using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dmarc.MxSecurityTester.Contract.Messages
{
    public class CertificateResultMessage
    {
        public CertificateResultMessage(string domain, List<HostInfo> hosts)
        {
            Hosts = hosts;
            Domain = domain;
        }

        public string Domain { get; }
        public List<HostInfo> Hosts { get; }
    }

    public class HostInfo
    {
        public HostInfo(string hostName, List<string> certificates = null)
        {
            HostName = hostName;
            Certificates = certificates?.Distinct().ToList() ?? new List<string>();
        }

        public string HostName { get; }
        public List<string> Certificates { get; }
    }
}
