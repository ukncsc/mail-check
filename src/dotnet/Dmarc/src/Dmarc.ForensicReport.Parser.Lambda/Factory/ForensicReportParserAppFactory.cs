using System;
using Dmarc.Common.Data;
using Dmarc.Common.Environment;
using Dmarc.Common.Logging;
using Dmarc.Common.Report.Conversion;
using Dmarc.Common.Report.Email;
using Dmarc.Common.Report.File;
using Dmarc.Common.Report.Parser;
using Dmarc.Common.Report.Persistance;
using Dmarc.Common.Report.Persistance.Dao;
using Dmarc.ForensicReport.Parser.Lambda.Console;
using Dmarc.ForensicReport.Parser.Lambda.Converters;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ContentType;
using Dmarc.ForensicReport.Parser.Lambda.Dao.EmailAddress;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinary;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinaryContent;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicBinaryHash;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicReport;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicReportUri;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicText;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextContent;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextContentUri;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicTextHash;
using Dmarc.ForensicReport.Parser.Lambda.Dao.ForensicUri;
using Dmarc.ForensicReport.Parser.Lambda.Dao.IpAddress;
using Dmarc.ForensicReport.Parser.Lambda.Dao.OriginalMailFrom;
using Dmarc.ForensicReport.Parser.Lambda.Dao.OriginalRcptTo;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822Header;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderField;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderSet;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Rfc822HeaderTextValue;
using Dmarc.ForensicReport.Parser.Lambda.Domain;
using Dmarc.ForensicReport.Parser.Lambda.Hashing;
using Dmarc.ForensicReport.Parser.Lambda.Parsers;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Common.Converters.ReceivedHeader;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.FeedbackReport;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.HumanReadable;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Body;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Body.EmailParts;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.MulitpartReport.Rfc822.Headers;
using Dmarc.ForensicReport.Parser.Lambda.Parsers.Urls;
using Microsoft.Extensions.DependencyInjection;

namespace Dmarc.ForensicReport.Parser.Lambda.Factory
{
    internal static class ForensicReportParserAppFactory 
    {
        public static IFileEmailMessageProcessor Create()
        {
            IServiceProvider provider = new ServiceCollection()
                .AddTransient<Common.Interface.Logging.ILogger, ConsoleLogger>()
                .AddTransient<IReportParser<ForensicReportInfo>, ForensicReportParser>()
                .AddTransient<IMimeMessageFactory, MimeMessageFactory>()
                .AddTransient<IFileEmailMessageProcessor, FileEmailMessageProcessor<ForensicReportInfo>>()
                .AddTransient<IMultipartReportParser, MultipartReportParser>()
                .AddTransient<IForensicReportTextPartParser, ForensicReportTextPartParser>()
                .AddTransient<ITextPartParser, TextPartParser>()
                .AddTransient<IMimePartParser, MimePartParser>()
                .AddTransient<IMultipartParser, MultipartParser>()
                .AddTransient<IHashInfoCalculator, Md5HashInfoCalculator>()
                .AddTransient<IHashInfoCalculator, Sha1HashInfoCalculator>()
                .AddTransient<IRfc822BodyParser, Rfc822BodyParser>()
                .AddTransient<IDateTimeConverter, DateTimeConverter>()
                .AddTransient<IFeedbackTypeConverter, FeedbackTypeConverter>()
                .AddTransient<IDeliveryResultConverter, DeliveryResultConverter>()
                .AddTransient<IAuthFailureConverter, AuthFailureConverter>()
                .AddTransient<IMailAddressCollectionConverter, MailAddressCollectionConverter>()
                .AddTransient<IMailAddressConverter, MailAddressConverter>()
                .AddTransient<IIPAddressConverter, IPAddressConverter>()
                .AddTransient<IIntConverter, IntConverter>()
                .AddTransient<IDateTimeParser, DateTimeParser>()
                .AddTransient<IMailAddressCollectionConverter, MailAddressCollectionConverter>()
                .AddTransient<IMailAddressParserMulti, MailAddressParserMulti>()
                .AddTransient<IMailAddressCollectionParser, MailAddressCollectionParser>()
                .AddTransient<IRawValueParser, RawValueParser>()
                .AddTransient<IRawValueParserMulti, RawValueParserMulti>()
                .AddTransient<IAuthFailureParser, AuthFailureParser>()
                .AddTransient<IBase64Parser, Base64Parser>()
                .AddTransient<IDeliveryResultParser, DeliveryResultParser>()
                .AddTransient<IFeedbackTypeParser, FeedbackTypeParser>()
                .AddTransient<IIntParser, IntParser>()
                .AddTransient<IIpAddressParser, IpAddressParser>()
                .AddTransient<IFeedbackReportParser, FeedbackReportParser>()
                .AddTransient<IDateTimeParserMulti, DateTimeParserMulti>()
                .AddTransient<IMailAddressCollectionParserMulti, MailAddressCollectionParserMulti>()
                .AddTransient<IMailAddressParser, MailAddressParser>()
                .AddTransient<IXOriginatingIPAddressParser, XOriginatingIPAddressParser>()
                .AddTransient<IRfc822HeadersParser, Rfc822HeadersParser>()
                .AddTransient<IUrlExtractor, UrlExtractor>()
                .AddTransient<IReportPersistor<ForensicReportInfo>, ForensicReportAppPersistor>()
                .AddTransient<IReceivedHeaderParserMulti, ReceivedHeaderParserMulti>()
                .AddTransient<IReceivedHeaderConverter, ReceivedHeaderConverter>()
                .AddTransient<IReceivedHeaderSplitter, ReceivedHeaderSplitter>()
                .AddTransient<IForensicReportEmailAddressToEntityConverter, ForensicReportEmailAddressToEntityConverter>()
                .AddTransient<IToEntityConverter<ForensicReportInfo, ForensicReportEntity>, ForensicReportToEntityConverter>()
                .AddTransient<IForensicReportUriToEntityConverter, ForensicReportUriToEntityConverter>()
                .AddTransient<IForensicUriToEntityConverter, ForensicUriToEntityConverter>()
                .AddTransient<IIpAddressToEntityConverter, IpAddressToEntityConverter>()
                .AddTransient<IMimeContentConverter, MimeContentConverter>()
                .AddTransient<IRfc822ToEntityConverter, Rfc822ToEntityConverter>()
                .AddTransient<ITextContentToEntityConverter, TextContentToEntityConverter>()
                .AddTransient<IContentTypeDao, ContentTypeDao>()
                .AddTransient<IEmailAddressDao, EmailAddressDao>()
                .AddTransient<IForensicBinaryDao, ForensicBinaryDao>()
                .AddTransient<IForensicBinaryContentDao, ForensicBinaryContentDao>()
                .AddTransient<IForensicBinaryHashDao, ForensicBinaryHashDao>()
                .AddTransient<IReportDaoAsync<ForensicReportEntity>, ForensicReportDao>()
                .AddTransient<IForensicReportUriDao, ForensicReportUriDao>()
                .AddTransient<IForensicTextDao, ForensicTextDao>()
                .AddTransient<IForensicTextContentDao, ForensicTextContentDao>()
                .AddTransient<IForensicTextContentUriDao, ForensicTextContentUriDao>()
                .AddTransient<IForensicTextHashDao, ForensicTextHashDao>()
                .AddTransient<IForensicUriDao, ForensicUriDao>()
                .AddTransient<IIpAddressDao, IpAddressDao>()
                .AddTransient<IOriginalMailFromDao, OriginalMailFromDao>()
                .AddTransient<IOrginalRcptToDao, OriginalRcptToDao>()
                .AddTransient<IRfc822HeaderDao, Rfc822HeaderDao>()
                .AddTransient<IRfc822HeaderFieldDao, Rfc822HeaderFieldDao>()
                .AddTransient<IRfc822HeaderSetDao, Rfc822HeaderSetDao>()
                .AddTransient<IRfc822HeaderTextValueDao, Rfc822HeaderTextValueDao>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddTransient<IConnectionInfo>(p => new StringConnectionInfo(Environment.GetEnvironmentVariable("ConnectionString")))
                .BuildServiceProvider();

            return provider.GetService<IFileEmailMessageProcessor>();
        }
    }
}
