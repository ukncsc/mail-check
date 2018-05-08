using System;

namespace Dmarc.MxSecurityTester.Dao.Entities
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

        protected bool Equals(Certificate other)
        {
            return string.Equals(Thumbprint, other.Thumbprint) && 
                string.Equals(Issuer, other.Issuer) && 
                string.Equals(Subject, other.Subject) && 
                StartDate.Equals(other.StartDate) && 
                EndDate.Equals(other.EndDate) && 
                KeyLength == other.KeyLength && 
                string.Equals(Algorithm, other.Algorithm) && 
                string.Equals(SerialNumber, other.SerialNumber) && 
                Version == other.Version && 
                Valid == other.Valid;
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Certificate) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Thumbprint?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (Issuer?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Subject?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ StartDate.GetHashCode();
                hashCode = (hashCode * 397) ^ EndDate.GetHashCode();
                hashCode = (hashCode * 397) ^ KeyLength;
                hashCode = (hashCode * 397) ^ (Algorithm?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (SerialNumber?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Version;
                hashCode = (hashCode * 397) ^ Valid.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{nameof(Thumbprint)}: {Thumbprint}{Environment.NewLine}" +
                   $"{nameof(Issuer)}: {Issuer}{Environment.NewLine}" +
                   $"{nameof(Subject)}: {Subject}{Environment.NewLine}" +
                   $"{nameof(StartDate)}: {StartDate}{Environment.NewLine}" +
                   $"{nameof(EndDate)}: {EndDate}{Environment.NewLine}" +
                   $"{nameof(KeyLength)}: {KeyLength}{Environment.NewLine}" +
                   $"{nameof(Algorithm)}: {Algorithm}{Environment.NewLine}" +
                   $"{nameof(SerialNumber)}: {SerialNumber}{Environment.NewLine}" +
                   $"{nameof(Version)}: {Version}{Environment.NewLine}" +
                   $"{nameof(Valid)}: {Valid}";
        }
    }
}