using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;

namespace Dmarc.ForensicReport.Parser.Lambda.Converters
{
    public interface IForensicReportUriToEntityConverter
    {
        ForensicReportUriEntity Convert(string uri);
    }

    public class ForensicReportUriToEntityConverter : IForensicReportUriToEntityConverter
    {
        private readonly IForensicUriToEntityConverter _forensicUriToEntityConverter;

        public ForensicReportUriToEntityConverter(IForensicUriToEntityConverter forensicUriToEntityConverter)
        {
            _forensicUriToEntityConverter = forensicUriToEntityConverter;
        }

        public ForensicReportUriEntity Convert(string uri)
        {
            return new ForensicReportUriEntity(_forensicUriToEntityConverter.Convert(uri));
        }
    }
}