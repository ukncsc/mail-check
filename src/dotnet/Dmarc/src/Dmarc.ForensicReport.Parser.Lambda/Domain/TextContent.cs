using System;
using System.Collections.Generic;
using System.Linq;

namespace Dmarc.ForensicReport.Parser.Lambda.Domain
{
    public class TextContent : EmailPart
    {
        public TextContent(string contentType, int depth, Disposition disposition, string rawContent, List<HashInfo> hashes, List<string> urls)
            : base(contentType, depth, disposition)
        {
            RawContent = rawContent;
            Hashes = hashes;
            Urls = urls;
        }

        public string RawContent { get; }
        public List<HashInfo> Hashes { get; }
        public List<string> Urls { get; }

        public override string ToString()
        {
            string indent = string.Join(string.Empty, Enumerable.Range(0, Depth).Select(_ => "\t"));

            string urls = $"{Environment.NewLine}{indent}Urls:{Environment.NewLine}{indent} {string.Join($"{Environment.NewLine}{indent} ", Urls)}";
                return $"{base.ToString()}{urls}";
        }
    }
}