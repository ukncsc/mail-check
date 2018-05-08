using System;
using System.Security.Cryptography;
using System.Text;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;

namespace Dmarc.ForensicReport.Parser.Lambda.Converters
{
    public interface IForensicUriToEntityConverter
    {
        ForensicUriEntity Convert(string uri);
    }

    public class ForensicUriToEntityConverter : IForensicUriToEntityConverter
    {
        public ForensicUriEntity Convert(string uri)
        {
            return new ForensicUriEntity(uri, CalculateHash(uri));
        }

        private string CalculateHash(string url)
        {
            using (HashAlgorithm hashAlgorithm = SHA256.Create())
            {
                return BitConverter.ToString(hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(url)))
                        .Replace("-", string.Empty)
                        .ToLower();
            }
        }
    }
}
