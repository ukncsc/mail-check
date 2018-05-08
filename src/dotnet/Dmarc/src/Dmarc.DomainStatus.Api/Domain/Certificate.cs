using System;
using System.Text.RegularExpressions;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class Certificate
    {
        public Certificate(string thumbPrint, 
            string issuer,
            string subject,
            DateTime startDate,
            DateTime endDate,
            string algorithm,
            string serialNumber,
            int version,
            bool valid)
        {
            ThumbPrint = thumbPrint;
            Issuer = issuer;
            Subject = subject;
            StartDate = startDate;
            EndDate = endDate;
            Algorithm = algorithm;
            SerialNumber = serialNumber;
            Version = version;
            Valid = valid;
            Name = Regex.Match(Subject, "(?<=^CN=)([^,]*(?=,))").Value;
        }

        public string ThumbPrint { get; }
        public string Issuer { get; }
        public string Subject { get; }
        public string Name { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public string Algorithm { get; }
        public string SerialNumber { get; }
        public int Version { get; }
        public bool Valid { get; }
    }
}