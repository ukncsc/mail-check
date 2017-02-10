using System;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using Dmarc.AggregateReport.Parser.Common.Compression;
using Dmarc.AggregateReport.Parser.Common.Email;
using Dmarc.AggregateReport.Parser.Common.Parsers;
using Dmarc.AggregateReport.Parser.Common.Serialisation.AggregateReportDeserialisation;
using Dmarc.AggregateReport.Parser.Lambda.Config;
using Dmarc.AggregateReport.Parser.Lambda.Converters;
using Dmarc.AggregateReport.Parser.Lambda.Dao;
using Dmarc.AggregateReport.Parser.Lambda.Email;
using Dmarc.AggregateReport.Parser.Lambda.Parser;
using Dmarc.AggregateReport.Parser.Lambda.QueueProcessing;
using Dmarc.Common.Data;
using Dmarc.Common.Environment;
using Dmarc.Common.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Dmarc.AggregateReport.Parser.Lambda.Factory
{
    internal static class AggregateReportParserLambdaFactory
    {
        public static IQueueProcessor Create(ILogger log)
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                .AddTransient<IAmazonS3>(provider => new AmazonS3Client(new EnvironmentVariablesAWSCredentials()))
                .AddTransient<ILambdaAggregateReportParserConfig, LambdaAggregateReportParserConfig>()
                .AddTransient<IAggregateReportDao, AggregateReportParserDao>()
                .AddTransient<IConnectionInfo>(p => new StringConnectionInfo(Environment.GetEnvironmentVariable("ConnectionString")))
                .AddTransient<IMimeMessageFactory, MimeMessageFactory>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddSingleton(log)
                .AddTransient<IGZipDecompressor, GZipDecompressor>()
                .AddTransient<IS3EmailMessageClient, S3EmailMessageClient>()
                .AddTransient<IZipDecompressor, ZipDecompressor>()
                .AddTransient<IAggregateReportDeserialiser, AggregateReportDeserialiser>()
                .AddTransient<IAttachmentStreamNormaliser, AttachmentStreamNormaliser>()
                .AddTransient<IAggregateReportParserAsync, AggregateReportParserLambda>()
                .AddTransient<IReportMetadataDeserialiser, ReportMetadataDeserialiser>()
                .AddTransient<IPolicyPublishedDeserialiser, PolicyPublishedDeserialiser>()
                .AddTransient<IRecordDeserialiser, RecordDeserialiser>()
                .AddTransient<IAuthResultDeserialiser, AuthResultDeserialiser>()
                .AddTransient<IDkimAuthResultDeserialiser, DkimAuthResultDeserialiser>()
                .AddTransient<IIdentifiersDeserialiser, IdentifiersDeserialiser>()
                .AddTransient<IPolicyEvaluatedDeserialiser, PolicyEvaluatedDeserialiser>()
                .AddTransient<IPolicyOverrideReasonDeserialiser, PolicyOverrideReasonDeserialiser>()
                .AddTransient<IRowDeserialiser, RowDeserialiser>()
                .AddTransient<IDomainValidator, DomainValidator>()
                .AddTransient<ISpfAuthResultDeserialiser, SpfAuthResultDeserialiser>()
                .AddTransient<IAggregateReportToEntityConverter, AggregateReportToEntityConverter>()
                .AddTransient<IMessageProcessor, S3EventMessageProcessor>()
                .AddTransient<IS3EmailMessageProcessor, S3EmailMessageProcessor>()
                .AddTransient<IS3EventDeserializer, S3EventDeserializer>()
                .AddTransient<IQueueProcessor, QueueProcessor>()
                .AddTransient<IAmazonSQS>(provider => new AmazonSQSClient(new EnvironmentVariablesAWSCredentials()))
                .BuildServiceProvider();

            return serviceProvider.GetService<IQueueProcessor>();
        }
    }
}
