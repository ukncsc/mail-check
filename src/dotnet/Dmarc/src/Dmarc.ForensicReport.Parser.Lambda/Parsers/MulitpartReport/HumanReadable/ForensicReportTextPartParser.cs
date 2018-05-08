using System;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Body.EmailParts;
using MimeKit;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.HumanReadable
{
    public interface IForensicReportTextPartParser
    {
        TextContent Parse(MimeEntity mimeEntity, int depth);
    }

    public class ForensicReportTextPartParser : IForensicReportTextPartParser
    {
        private readonly ITextPartParser _textPartParser;

        public ForensicReportTextPartParser(ITextPartParser textPartParser)
        {
            _textPartParser = textPartParser;
        }

        public TextContent Parse(MimeEntity mimeEntity, int depth)
        {
            TextPart textPart = mimeEntity as TextPart;
            if (textPart == null)
            {
                throw new ArgumentException($"Expected Mime Part 1 to be {typeof(TextPart)} but  was {mimeEntity.GetType()}.");
            }

            if (!textPart.ContentType.IsMimeType(MimeTypes.Text, MimeTypes.Plain))
            {
                throw new ArgumentException($"Expected ContentType to be {MimeTypes.Text}/{MimeTypes.Plain} but was {textPart.ContentType.MimeType}.");
            }

            return _textPartParser.Parse(textPart, depth);
        }
    }
}