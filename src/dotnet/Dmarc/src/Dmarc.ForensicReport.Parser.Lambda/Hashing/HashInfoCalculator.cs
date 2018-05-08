using System;
using System.IO;
using System.Security.Cryptography;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using MimeKit;
using MimeKit.IO;
using MimeKit.IO.Filters;

namespace Dmarc.ForensicReport.Parser.Lambda.Hashing
{
    public abstract class HashInfoCalculator
    {
        protected HashInfo Calculate(MimePart mimePart, Func<HashAlgorithm> hashAlgorithmFactory, HashType hashType)
        {        
            using (Stream stream = mimePart.ContentObject.Open())
            {
                using (FilteredStream filteredStream = new FilteredStream(stream))
                {
                    if (mimePart.ContentType.IsMimeType("text", "*"))
                    {
                        filteredStream.Add(new Unix2DosFilter());
                    }
            
                    using (HashAlgorithm hashAlgorithm = hashAlgorithmFactory())
                    {
                        return new HashInfo
                        {
                            Hash = BitConverter.ToString(hashAlgorithm.ComputeHash(filteredStream)).Replace("-", string.Empty).ToLower(),
                            HashType = hashType
                        };
                    }
                }
            }               
        }
    }
}