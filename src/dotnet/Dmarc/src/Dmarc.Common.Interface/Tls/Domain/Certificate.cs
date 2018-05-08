using System;

namespace Dmarc.Common.Interface.Tls.Domain
{
    public class Certificate
    {
        public Certificate(
            string thumbprint, //id
            string issuer,
            string subject,
            DateTime startDate,
            DateTime endDate,
            int keyLength,
            string algorithm,
            string serialNumber,
            int version,
            bool valid
        )
        {
            Thumbprint = thumbprint;
            Issuer = issuer;
            Subject = subject;
            StartDate = startDate;
            EndDate = endDate;
            KeyLength = keyLength;
            Algorithm = algorithm;
            SerialNumber = serialNumber;
            Version = version;
            Valid = valid;
        }

        public string Thumbprint { get; }
        public string Issuer { get; }
        public string Subject { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public int KeyLength { get; }
        public string Algorithm { get; }
        public string SerialNumber { get; }
        public int Version { get; }
        public bool Valid { get; }
    }
}