using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using MimeKit;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers
{
    public interface IRfc822HeadersParser
    {
        Domain.Rfc822 Parse(MimeEntity mimeEntity, int depth);
    }

    public class Rfc822HeadersParser : IRfc822HeadersParser
    {
        private const string Date = "date";
        private const string ResentDate = "date-date";
        private const string Received = "received";
        private const string ReturnPath = "return-path";
        private const string From = "from";
        private const string ResentFrom = "resent-from";
        private const string Sender = "sender";
        private const string ResentSender = "resent-sender";
        private const string ReplyTo = "reply-to";
        private const string To = "to";
        private const string ResentTo = "resent-to";
        private const string Cc = "cc";
        private const string ResentCc = "resent-cc";
        private const string Bcc = "bcc";
        private const string ResentBcc = "resent-bcc";
        private const string MessagId = "message-id";
        private const string ResentMessagId = "resent-message-id";
        private const string InReplyTo = "in-reply-to";
        private const string References = "refernces";
        private const string Subject = "subject";
        private const string Comment = "comments";
        private const string Keyword = "keywords";
        private const string AuthenticationResults = "authentication-results";
        private const string ReceivedSpf = "received-spf";
        private const string UserAgent = "user-agent";
        private const string DkimSignature = "dkim-Signature";
        private const string XOriginatingIp = "x-originating-ip";

        private readonly IMailAddressCollectionParser _mailAddressCollectionParser;
        private readonly IMailAddressCollectionParserMulti _mailAddressCollectionParserMulti;
        private readonly IMailAddressParserMulti _mailAddressParserMulti;
        private readonly IMailAddressParser _mailAddressParser;
        private readonly IRawValueParserMulti _rawValueParserMulti;
        private readonly IRawValueParser _rawValueParser;
        private readonly IDateTimeParserMulti _dateTimeParserMulti;
        private readonly IDateTimeParser _dateTimeParser;
        private readonly IXOriginatingIPAddressParser _xOriginatingIpAddressParser;
        private readonly IReceivedHeaderParserMulti _receivedHeaderParserMulti;

        public Rfc822HeadersParser(IMailAddressCollectionParser mailAddressCollectionParser,
            IMailAddressCollectionParserMulti mailAddressCollectionParserMulti,
            IMailAddressParserMulti mailAddressParserMulti,
            IMailAddressParser mailAddressParser,
            IRawValueParserMulti rawValueParserMulti,
            IRawValueParser rawValueParser,
            IDateTimeParserMulti dateTimeParserMulti,
            IDateTimeParser dateTimeParser,
            IXOriginatingIPAddressParser xOriginatingIpAddressParser,
            IReceivedHeaderParserMulti receivedHeaderParserMulti)
        {
            _mailAddressCollectionParser = mailAddressCollectionParser;
            _mailAddressCollectionParserMulti = mailAddressCollectionParserMulti;
            _mailAddressParserMulti = mailAddressParserMulti;
            _mailAddressParser = mailAddressParser;
            _rawValueParserMulti = rawValueParserMulti;
            _rawValueParser = rawValueParser;
            _dateTimeParserMulti = dateTimeParserMulti;
            _dateTimeParser = dateTimeParser;
            _xOriginatingIpAddressParser = xOriginatingIpAddressParser;
            _receivedHeaderParserMulti = receivedHeaderParserMulti;
        }

        public Domain.Rfc822 Parse(MimeEntity mimeEntity, int depth)
        {
            if (mimeEntity.ContentType.IsMimeType(MimeTypes.Message, MimeTypes.Rfc822))
            {
                MessagePart messagePart = mimeEntity as MessagePart;
                if (messagePart != null)
                {
                    Disposition disposition = mimeEntity.ContentDisposition == null
                        ? null
                        : new Disposition(mimeEntity.ContentDisposition.IsAttachment, mimeEntity.ContentDisposition.FileName);

                    return Parse(messagePart.Message.Headers, mimeEntity.ContentType.MimeType, depth, disposition);
                }
                throw new ArgumentException($"Expected MimeEntity to be {typeof(MessagePart)} but was {mimeEntity.GetType()}.");
            }

            if (mimeEntity.ContentType.IsMimeType(MimeTypes.Text, MimeTypes.Rfc822Headers))
            {
                TextPart textPart = mimeEntity as TextPart;
                if (textPart != null)
                {
                    using (Stream stream = textPart.ContentObject.Open())
                    {
                        MimeParser parser = new MimeParser(ParserOptions.Default, stream, MimeFormat.Entity);

                        Disposition disposition = mimeEntity.ContentDisposition == null
                            ? null
                            : new Disposition(mimeEntity.ContentDisposition.IsAttachment, mimeEntity.ContentDisposition.FileName);

                        return Parse(parser.ParseHeaders(), mimeEntity.ContentType.MimeType, depth, disposition);
                    }
                }
                throw new ArgumentException($"Expected {nameof(mimeEntity)} to derive from {typeof(TextPart)}.");
            }

            throw new ArgumentException($"Expected ContentType to be {MimeTypes.Message}/{MimeTypes.Rfc822} or {MimeTypes.Text}/{MimeTypes.Rfc822Headers} but was {mimeEntity.ContentType.MimeType}.");
        }

        private Domain.Rfc822 Parse(HeaderList headerList, string contentType, int depth, Disposition disposition)
        {
            Dictionary<string, List<string>> headers = headerList
                   .GroupBy(_ => _.Field.ToLower())
                   .ToDictionary(_ => _.Key, h => h.Select(_ => _.Value).ToList());

            return new Domain.Rfc822(contentType, depth, disposition)
            {
                Date = _dateTimeParser.Parse(headers, Date, true, true),
                ResentDate = _dateTimeParserMulti.Parse(headers, ResentDate),
                Received =  _receivedHeaderParserMulti.Parse(headers, Received),
                ReturnPath = _mailAddressParserMulti.Parse(headers, ReturnPath),
                From = _mailAddressCollectionParser.Parse(headers, From),
                ResentFrom = _mailAddressCollectionParserMulti.Parse(headers, ResentFrom),
                Sender = _mailAddressParser.Parse(headers, Sender),
                ResentSender = _mailAddressCollectionParserMulti.Parse(headers, ResentSender),
                To = _mailAddressCollectionParser.Parse(headers, To), //this is a slight relaxation of the spec as this allows groups as well as addresses.
                ResentTo = _mailAddressCollectionParserMulti.Parse(headers, ResentTo), //this is a slight relaxation of the spec as this allows groups as well as addresses.
                ReplyTo = _mailAddressCollectionParser.Parse(headers, ReplyTo), //this is a slight relaxation of the spec as this allows groups as well as addresses.
                Cc = _mailAddressCollectionParser.Parse(headers, Cc), //this is a slight relaxation of the spec as this allows groups as well as addresses.
                ResentCc = _mailAddressCollectionParserMulti.Parse(headers, ResentCc), //this is a slight relaxation of the spec as this allows groups as well as addresses.
                Bcc = _mailAddressCollectionParser.Parse(headers, Bcc), //this is a slight relaxation of the spec as this allows groups as well as addresses.
                ResentBcc = _mailAddressCollectionParserMulti.Parse(headers, ResentBcc), //this is a slight relaxation of the spec as this allows groups as well as addresses.
                MessageId = _mailAddressParser.Parse(headers, MessagId), //The message identifier (msg-id) syntax is a limited version of the addr-spec
                ResentMessageId = _mailAddressParserMulti.Parse(headers, ResentMessagId),
                InReplyTo = _mailAddressCollectionParser.Parse(headers, InReplyTo),
                References = _mailAddressCollectionParser.Parse(headers, References),
                Subject = _rawValueParser.Parse(headers, Subject),
                Comments = _rawValueParserMulti.Parse(headers, Comment),
                Keywords = _rawValueParserMulti.Parse(headers, Keyword),
                AuthenticationResults = _rawValueParser.Parse(headers, AuthenticationResults),
                ReceivedSpf = _rawValueParser.Parse(headers, ReceivedSpf),
                UserAgent = _rawValueParser.Parse(headers, UserAgent),
                DkimSignature = _rawValueParser.Parse(headers, DkimSignature),
                XOrigninatingIp = _xOriginatingIpAddressParser.Parse(headers, XOriginatingIp)
            };
        }
    }
}