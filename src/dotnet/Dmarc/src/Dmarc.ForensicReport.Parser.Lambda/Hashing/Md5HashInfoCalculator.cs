using System.Security.Cryptography;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using MimeKit;

namespace Dmarc.ForensicReport.Parser.Lambda.Hashing
{
    public class Md5HashInfoCalculator : HashInfoCalculator, IHashInfoCalculator
    {
        public HashInfo Calculate(MimePart mimePart)
        {
            return Calculate(mimePart, MD5.Create, HashType.Md5);
        }
    }
}