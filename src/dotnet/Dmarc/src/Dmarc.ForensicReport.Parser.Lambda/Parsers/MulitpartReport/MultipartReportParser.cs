using System;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using MimeKit;
using Multipart = Dmarc.ForensicReport.Parser.Lambda.Domain.Multipart;

namespace Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport
{
    public interface IMultipartReportParser
    {
        Multipart Parse(MimeEntity mimeEntity, int depth);
    }

    public class MultipartReportParser : IMultipartReportParser
    {
        public Multipart Parse(MimeEntity mimeEntity, int depth)
        {
            MultipartReport multipartReport = mimeEntity as MultipartReport;

            //rfc5965 section 2 a - The "report-type" parameter of the "multipart/report" type is set
            //to "feedback-report";
            if (multipartReport == null)
            {
                throw new ArgumentException($"Expected {typeof(MultipartReport)} but found {mimeEntity.ContentType.MimeType}.");
            }

            if (multipartReport.ReportType.ToLower() != MimeTypes.FeedbackReport)
            {
                throw new ArgumentException($"Expected ReportType to be {MimeTypes.FeedbackReport} but was {multipartReport.ReportType}.");
            }

            if (multipartReport.Count != 3)
            {
                throw new ArgumentException($"Expected 3 Mime Parts in {multipartReport.ContentType} but found {multipartReport.Count}.");
            }

            Disposition disposition = mimeEntity.ContentDisposition == null
                ? null
                : new Disposition(mimeEntity.ContentDisposition.IsAttachment, mimeEntity.ContentDisposition.FileName);

            return new Multipart(multipartReport.ContentType.MimeType, depth, disposition);
        }
    }
}