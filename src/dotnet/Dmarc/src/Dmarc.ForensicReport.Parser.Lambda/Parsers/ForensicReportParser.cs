using System;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.Common.Report.Domain;
using Dmarc.Common.Report.Email;
using Dmarc.Common.Report.Parser;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.FeedbackReport;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.HumanReadable;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Body;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers;
using MimeKit;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers
{
    public class ForensicReportParser : IReportParser<ForensicReportInfo>
    {
        private readonly IMimeMessageFactory _mimeMessageFactory;
        private readonly IMultipartReportParser _multipartReportParser;
        private readonly IForensicReportTextPartParser _forensicReportTextPartParser;
        private readonly IFeedbackReportParser _feedbackReportParser;
        private readonly IRfc822HeadersParser _rfc822HeadersParser;
        private readonly IRfc822BodyParser _rfc822BodyParser;
        private readonly ILogger _log;

        public ForensicReportParser(IMimeMessageFactory mimeMessageFactory,
            IMultipartReportParser multipartReportParser,
            IForensicReportTextPartParser forensicReportTextPartParser,
            IFeedbackReportParser feedbackReportParser,
            IRfc822HeadersParser rfc822HeadersParser,
            IRfc822BodyParser rfc822BodyParser,
            ILogger log)
        {
            _mimeMessageFactory = mimeMessageFactory;
            _multipartReportParser = multipartReportParser;
            _forensicReportTextPartParser = forensicReportTextPartParser;
            _feedbackReportParser = feedbackReportParser;
            _rfc822HeadersParser = rfc822HeadersParser;
            _rfc822BodyParser = rfc822BodyParser;
            _log = log;
        }

        public ForensicReportInfo Parse(EmailMessageInfo messageInfo)
        {
            MimeMessage mimeMessage = _mimeMessageFactory.Create(messageInfo.EmailStream);

            using (MimeIterator iterator = new MimeIterator(mimeMessage))
            {
                if (!iterator.MoveNext())
                {
                    throw new ArgumentException("Expected to find mime part but didn't.");
                }

                Domain.ForensicReport forensicReport = new Domain.ForensicReport(mimeMessage.MessageId);

                forensicReport.Add(_multipartReportParser.Parse(iterator.Current, iterator.Depth));

                //rfc5965 section 2 b
                if (!iterator.MoveNext())
                {
                    throw new ArgumentException("Expected to find mime part but didn't.");
                }

                forensicReport.Add(_forensicReportTextPartParser.Parse(iterator.Current, iterator.Depth));

                if (!iterator.MoveNext())
                {
                    throw new ArgumentException("Expected to find mime part but didn't.");
                }

                forensicReport.Add(_feedbackReportParser.Parse(iterator.Current, iterator.Depth));

                if (!iterator.MoveNext())
                {
                    throw new ArgumentException("Expected to find mime part but didn't.");
                }

                forensicReport.Add(_rfc822HeadersParser.Parse(iterator.Current, iterator.Depth));

                while (iterator.MoveNext())
                {
                    forensicReport.Add(_rfc822BodyParser.Parse(iterator.Current, iterator.Depth));
                }

                _log.Trace(forensicReport.ToString());

                return new ForensicReportInfo(forensicReport, messageInfo.EmailMetadata);
            }
        }
    }
}
