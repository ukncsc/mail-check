using System.Security.Cryptography;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using MimeKit;

namespace Dmarc.ForensicReport.Parser.Lambda.Hashing
{
    public class Sha1HashInfoCalculator : HashInfoCalculator, IHashInfoCalculator
    {
        public HashInfo Calculate(MimePart mimePart)
        {
            return Calculate(mimePart, SHA1.Create, HashType.Sha1);
        }
    }
}