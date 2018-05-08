using System;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class MxRecord : DnsRecord
    {
        public MxRecord(int id, 
            int? preference, 
            string hostname, 
            TlsResult sslv3,
            TlsResult tlsv1,
            TlsResult tlsv11,
            TlsResult tlsv12,
            Certificate certificate,
            DateTime? tlsLastChecked,
            DateTime? mxLastChecked)
            : base(id, mxLastChecked)
        {
            Preference = preference;
            Hostname = hostname;
            Sslv3 = sslv3;
            Tlsv1 = tlsv1;
            Tlsv11 = tlsv11;
            Tlsv12 = tlsv12;
            Certificate = certificate;
            TlsLastChecked = tlsLastChecked;
        }

        public int? Preference { get; }
        public string Hostname { get; }
        public TlsResult Sslv3 { get; }
        public TlsResult Tlsv1 { get; }
        public TlsResult Tlsv11 { get; }
        public TlsResult Tlsv12 { get; }
        public Certificate Certificate { get; }
        public DateTime? TlsLastChecked { get; }
    }
}