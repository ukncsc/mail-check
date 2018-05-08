using System;
using System.Collections.Generic;
using System.Linq;

namespace Dmarc.ForensicReport.Parser.Lambda.Domain
{
    public class MimeContent : EmailPart
    {
        public MimeContent(string contentType, int depth, Disposition disposition, byte[] rawContent, List<HashInfo> hashes) 
            : base(contentType, depth, disposition)
        {
            RawContent = rawContent;
            Hashes = hashes;
        }

        public byte[] RawContent { get; }
        public List<HashInfo> Hashes { get; }

        public override string ToString()
        {
            string indent = string.Join(string.Empty, Enumerable.Range(0, Depth).Select(_ => "\t"));
            return $"{base.ToString()}{Environment.NewLine}{indent}Hashes:{Environment.NewLine}{indent} {Hashes[0].HashType}:{Hashes[0].Hash}{Environment.NewLine}{indent} {Hashes[1].HashType}:{Hashes[1].Hash}";
        }
    }
}