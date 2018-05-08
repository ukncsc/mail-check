using System.Collections.Generic;
using System.Linq;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Hashing;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Urls;
using MimeKit;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Body.EmailParts
{
    public interface ITextPartParser
    {
        TextContent Parse(TextPart textPart, int depth);
    }

    public class TextPartParser : ITextPartParser
    {
        private readonly IEnumerable<IHashInfoCalculator> _hashInfoCalculators;
        private readonly IUrlExtractor _urlExtractor;

        public TextPartParser(IEnumerable<IHashInfoCalculator> hashInfoCalculators, IUrlExtractor urlExtractor)
        {
            _hashInfoCalculators = hashInfoCalculators;
            _urlExtractor = urlExtractor;
        }

        public TextContent Parse(TextPart textPart, int depth)
        {
            List<HashInfo> hashInfos = _hashInfoCalculators.Select(_ => _.Calculate(textPart)).ToList();
            List<string> urls = _urlExtractor.ExtractUrls(textPart.Text);

            Disposition disposition = textPart.ContentDisposition == null
                ? null
                : new Disposition(textPart.ContentDisposition.IsAttachment, textPart.ContentDisposition.FileName);

            return new TextContent(textPart.ContentType.MimeType, depth, disposition, textPart.Text, hashInfos, urls);
        }
    }
}