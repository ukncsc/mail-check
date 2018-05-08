using System;
using System.Collections.Generic;
using System.Linq;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers;
using MimeKit;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.FeedbackReport
{
    public interface IFeedbackReportParser
    {
        Domain.FeedbackReport Parse(MimeEntity mimeEntity, int depth);
    }

    public class FeedbackReportParser : IFeedbackReportParser
    {
        private const string FeedbackType = "feedback-type";
        private const string UserAgent = "user-agent";
        private const string Version = "version";
        private const string AuthFailure = "auth-failure";
        private const string OriginalEnvelopeId = "original-envelope-id";
        private const string OriginalMailFrom = "original-mail-from";
        private const string ArrivalDate = "arrival-date";
        private const string ReceivedDate = "received-date";
        private const string ReportingMta = "reporting-mta";
        private const string SourceIp = "source-ip";
        private const string Incidents = "incidents";
        private const string DeliveryResult = "delivery-result";
        private const string MessageId = "message-id";
        private const string DkimDomain = "dkim-domain";
        private const string DkimIdentity = "dkim-identity";
        private const string DkimSelector = "dkim-selector";
        private const string DkimCanonicalizedHeader = "dkim-canonicalized-header";
        private const string DkimCanonicalizedBody = "dkim-canonicalized-body";
        private const string SpfDns = "spf-dns";
        private const string AuthenticationResult = "authentication-results";
        private const string OriginalRcptTo = "original-rcpt-to";
        private const string ReportedDomain = "reported-domain";
        private const string ReportedUri = "reported-uri";

        private readonly IFeedbackTypeParser _feedbackTypeParser;
        private readonly IRawValueParser _rawValueParser;
        private readonly IAuthFailureParser _authFailureParser;
        private readonly IMailAddressCollectionParser _mailAddressCollectionParser;
        private readonly IDateTimeParser _dateTimeParser;
        private readonly IIntParser _intParser;
        private readonly IDeliveryResultParser _deliveryResultParser;
        private readonly IBase64Parser _base64Parser;
        private readonly IRawValueParserMulti _rawValueParserMulti;
        private readonly IMailAddressParser _mailAddressParser;
        private readonly IMailAddressParserMulti _mailAddressParserMulti;
        private readonly IIpAddressParser _ipAddressParser;

        public FeedbackReportParser(
            IFeedbackTypeParser feedbackTypeParser,
            IRawValueParser rawValueParser,
            IAuthFailureParser authFailureParser,
            IMailAddressCollectionParser mailAddressCollectionParser,
            IDateTimeParser dateTimeParser,
            IIntParser intParser,
            IDeliveryResultParser deliveryResultParser,
            IBase64Parser base64Parser,
            IRawValueParserMulti rawValueParserMulti,
            IMailAddressParser mailAddressParser,
            IMailAddressParserMulti mailAddressParserMulti,
            IIpAddressParser ipAddressParser)
        {
            _feedbackTypeParser = feedbackTypeParser;
            _rawValueParser = rawValueParser;
            _authFailureParser = authFailureParser;
            _mailAddressCollectionParser = mailAddressCollectionParser;
            _dateTimeParser = dateTimeParser;
            _intParser = intParser;
            _deliveryResultParser = deliveryResultParser;
            _base64Parser = base64Parser;
            _rawValueParserMulti = rawValueParserMulti;
            _mailAddressParser = mailAddressParser;
            _mailAddressParserMulti = mailAddressParserMulti;
            _ipAddressParser = ipAddressParser;
        }

        public Domain.FeedbackReport Parse(MimeEntity mimeEntity, int depth)
        {
            //rfc5965 2c.
            if (!mimeEntity.ContentType.IsMimeType(MimeTypes.Message, MimeTypes.FeedbackReport))
            {
                throw new ArgumentException($"Expected ContentType of { MimeTypes.Message}/{MimeTypes.FeedbackReport} but was {mimeEntity.ContentType.MimeType}.");
            }

            MimePart mimePart = mimeEntity as MimePart;
            if (mimePart != null)
            {
                MimeParser parser = new MimeParser(ParserOptions.Default, mimePart.ContentObject.Stream, MimeFormat.Entity);

                Dictionary<string, List<string>> headerList = parser.ParseHeaders()
                    .GroupBy(_ => _.Field.ToLower())
                    .ToDictionary(_ => _.Key, h => h.Select(_ => _.Value).ToList());

                Disposition disposition = mimeEntity.ContentDisposition == null
                    ? null
                    : new Disposition(mimeEntity.ContentDisposition.IsAttachment, mimeEntity.ContentDisposition.FileName);

                return new Domain.FeedbackReport(mimeEntity.ContentType.MimeType, depth, disposition)
                {                    
                    //rfc5965
                    //required single
                    FeedbackType = _feedbackTypeParser.Parse(headerList, FeedbackType, true, true),
                    UserAgent = _rawValueParser.Parse(headerList, UserAgent, true, true),
                    Version = _rawValueParser.Parse(headerList, Version), //relaxed to optional as not always populated

                    //single optional. Should also handle Received-Date as fallback
                    ArrivalDate = _dateTimeParser.Parse(headerList, ArrivalDate) ??
                        _dateTimeParser.Parse(headerList, ReceivedDate),

                    //single optional
                    ReportingMta = _rawValueParser.Parse(headerList, ReportingMta),
                    SourceIp = _ipAddressParser.Parse(headerList, SourceIp),
                    Indicents = _intParser.Parse(headerList, Incidents),

                    //multiple optional.
                    OrginalRcptTos = _mailAddressParserMulti.Parse(headerList, OriginalRcptTo),
                    ReportedUris = _rawValueParserMulti.Parse(headerList, ReportedUri),

                    //rfc6591
                    //required single. Relaxed to optional as not all reports populate this field
                    AuthFailure = _authFailureParser.Parse(headerList, AuthFailure),

                    //single optional.
                    OriginalEnvelopeId = _rawValueParser.Parse(headerList, OriginalEnvelopeId),
                    OriginalMailFrom = _mailAddressCollectionParser.Parse(headerList, OriginalMailFrom),
                    DeliveryResult = _deliveryResultParser.Parse(headerList, DeliveryResult),
                    AuthenticationResults = _rawValueParser.Parse(headerList, AuthenticationResult),
                    ReportedDomain = _rawValueParser.Parse(headerList, ReportedDomain),

                    MessageId = _mailAddressParser.Parse(headerList, MessageId), //The message identifier (msg-id) syntax is a limited version of the addr-spec

                    //single required for dkim report. Relaxing this to optional as we are not diffentiating report types.
                    DkimDomain = _rawValueParser.Parse(headerList, DkimDomain),

                    //single required for dkim report. Relaxing this to optional as we are not diffentiating report types.
                    DkimIdentity = _rawValueParser.Parse(headerList, DkimIdentity),
                    DkimSelector = _rawValueParser.Parse(headerList, DkimSelector),

                    //single optional for dkim report.
                    DkimCanonicalizedHeader = _base64Parser.Parse(headerList, DkimCanonicalizedHeader),
                    DkimCanonicalizedBody = _base64Parser.Parse(headerList, DkimCanonicalizedBody),

                    //single required for spf report. Relaxing this to optional as we are not diffentiating report types.
                    SpfDns = _rawValueParser.Parse(headerList, SpfDns)
                };
            }
            throw new ArgumentException($"Expected {nameof(mimeEntity)} to derive from {typeof(MimePart)}.");
        }
    }
}