using System;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Body.EmailParts;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers;
using MimeKit;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Body
{
    public interface IRfc822BodyParser
    {
        EmailPart Parse(MimeEntity mimeEntity, int depth);
    }

    public class Rfc822BodyParser : IRfc822BodyParser
    {
        private readonly IMultipartParser _multipartParser;
        private readonly IRfc822HeadersParser _headersParser;
        private readonly ITextPartParser _textPartParser;
        private readonly IMimePartParser _mimePartParser;

        public Rfc822BodyParser(IMultipartParser multipartParser,
            IRfc822HeadersParser headersParser,
            ITextPartParser textPartParser,
            IMimePartParser mimePartParser)
        {
            _multipartParser = multipartParser;
            _headersParser = headersParser;
            _textPartParser = textPartParser;
            _mimePartParser = mimePartParser;
        }

        public EmailPart Parse(MimeEntity mimeEntity, int depth)
        {
            if (mimeEntity.ContentType.IsMimeType(MimeTypes.Multipart, MimeTypes.Wildcard))
            {
                return _multipartParser.Parse(mimeEntity, depth);
            }

            var textPart = mimeEntity as TextPart;
            if (textPart != null)
            {
                return _textPartParser.Parse(textPart, depth);
            }

            var mimePart = mimeEntity as MimePart;
            if (mimePart != null)
            {
                return _mimePartParser.Parse(mimePart, depth);
            }

            var messagePart = mimeEntity as MessagePart;
            if (messagePart != null)
            {
                if (messagePart.ContentType.IsMimeType(MimeTypes.Message, MimeTypes.Rfc822) || 
                    messagePart.ContentType.IsMimeType(MimeTypes.Text, MimeTypes.Rfc822Headers))
                {
                    return _headersParser.Parse(mimeEntity, depth);
                }
            }
            
            throw new ArgumentException($"Unexpected type {mimeEntity.GetType()} with ContentType of {mimeEntity.ContentType.MimeType} for {nameof(mimeEntity)}");
        }
    }
}