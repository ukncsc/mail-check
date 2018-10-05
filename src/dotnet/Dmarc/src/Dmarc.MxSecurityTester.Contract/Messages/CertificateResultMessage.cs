using System;
using System.Collections.Generic;
using System.Linq;

namespace Dmarc.MxSecurityTester.Contract.Messages
{
    public class CertificateResultMessage
    {
        public CertificateResultMessage(string domain, List<HostInfo> hosts, DateTime? lastChecked = null)
        {
            Hosts = hosts;
            Domain = domain;
            LastChecked = lastChecked ?? DateTime.Now;
        }

        public string Domain { get; }

        public List<HostInfo> Hosts { get; }

        public DateTime LastChecked { get; }
    }

    public class HostInfo
    {
        public HostInfo(string hostName, bool hostNotFound, List<string> certificates = null, List<SelectedCipherSuite> selectedCipherSuites = null)
        {
            HostName = hostName;
            HostNotFound = hostNotFound;
            Certificates = certificates?.Distinct().ToList() ?? new List<string>();
            SelectedCipherSuites = selectedCipherSuites ?? new List<SelectedCipherSuite>();
        }

        public string HostName { get; }

        public List<string> Certificates { get; }

        public List<SelectedCipherSuite> SelectedCipherSuites { get; }

        public bool HostNotFound { get; }
    }

    public class SelectedCipherSuite
    {
        public SelectedCipherSuite(string testName, string cipherSuite)
        {
            TestName = testName;
            CipherSuite = cipherSuite;
        }

        public string TestName { get; }

        public string CipherSuite { get; }
    }
}
