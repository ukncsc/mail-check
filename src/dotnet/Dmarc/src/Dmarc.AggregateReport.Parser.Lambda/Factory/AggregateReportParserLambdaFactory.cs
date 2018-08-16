using System;
using System.Collections.Generic;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SimpleSystemsManagement;
using Amazon.SQS;
using Dmarc.AggregateReport.Parser.Lambda.Compression;
using Dmarc.AggregateReport.Parser.Lambda.Config;
using Dmarc.AggregateReport.Parser.Lambda.Converters;
using Dmarc.AggregateReport.Parser.Lambda.Dao;
using Dmarc.AggregateReport.Parser.Lambda.Dao.Entities;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Parser;
using Dmarc.AggregateReport.Parser.Lambda.Publishers;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation;
using Dmarc.Common.Data;
using Dmarc.Common.Environment;
using Dmarc.Common.Report.Config;
using Dmarc.Common.Report.Conversion;
using Dmarc.Common.Report.Email;
using Dmarc.Common.Report.Parser;
using Dmarc.Common.Report.Persistance.Dao;
using Dmarc.Common.Report.QueueProcessing;
using Dmarc.Common.Validation;
using Microsoft.Extensions.DependencyInjection;
using Dmarc.Common.Encryption;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.Messaging;
using Dmarc.Common.Messaging.Sns.Publisher;

namespace Dmarc.AggregateReport.Parser.Lambda.Factory
{
    internal static class AggregateReportParserLambdaFactory
    {
        public static IQueueProcessor Create(ILogger log)
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                .AddTransient<IAmazonS3>(provider => new AmazonS3Client(new EnvironmentVariablesAWSCredentials()))
                .AddTransient<ILambdaReportParserConfig, LambdaAggregateReportParserConfig>()
                .AddTransient<IPublisherConfig, LambdaAggregateReportParserConfig>()
                .AddTransient<IDkimSelectorPublisherConfig, DkimSelectorPublisherConfig>()
                .AddTransient<IReportDaoAsync<AggregateReportEntity>, AggregateReportParserDaoAsync>()
                .AddTransient<IAmazonSimpleSystemsManagement>(p => new AmazonSimpleSystemsManagementClient(new EnvironmentVariablesAWSCredentials()))
                .AddTransient<IParameterStoreRequest, ParameterStoreRequest>()
                .AddTransient<IConnectionInfo, LambdaAggregateReportParserConfig>()
                .AddSingleton<IConnectionInfoAsync, ConnectionInfoAsync>()
                .AddTransient<IMimeMessageFactory, MimeMessageFactory>()
                .AddTransient<IEnvironment, EnvironmentWrapper>()
                .AddTransient<IEnvironmentVariables, EnvironmentVariables>()
                .AddSingleton(log)
                .AddTransient<IGZipDecompressor, GZipDecompressor>()
                .AddTransient<IS3EmailMessageClient, S3EmailMessageClient>()
                .AddTransient<IEmailMessageInfoProcessor<AggregateReportInfo>, EmailMessageInfoProcessor<AggregateReportInfo>>()
                .AddS3EmailMessageProcessor()
                .AddTransient<IZipDecompressor, ZipDecompressor>()
                .AddTransient<IAggregateReportDeserialiser, AggregateReportDeserialiser>()
                .AddTransient<IAttachmentStreamNormaliser, AttachmentStreamNormaliser>()
                .AddTransient<IContentTypeProvider, ContentTypeProvider>()
                .AddTransient<IReportParser<AggregateReportInfo>, AggregateReportParser>()
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
                .AddTransient<IToEntityConverter<AggregateReportInfo, AggregateReportEntity>, AggregateReportToEntityConverter>()
                .AddTransient<IMessageProcessor, S3EventMessageProcessor>()
                .AddTransient<IS3EventDeserializer, S3EventDeserializer>()
                .AddTransient<IQueueProcessor, QueueProcessor>()
                .AddTransient<IAmazonSQS>(provider => new AmazonSQSClient(new EnvironmentVariablesAWSCredentials()))
                .AddTransient<IAmazonSimpleNotificationService>(provider => new AmazonSimpleNotificationServiceClient(new EnvironmentVariablesAWSCredentials()))
                .AddTransient<IPublisher, SnsPublisher>()
                .AddTransient<IMessagePublisher, DkimSelectorSeenPublisher>()
                .AddTransient<IMessagePublisher, AggregateReportIpAddressesMessagePublisher>()   
                .BuildServiceProvider();

            return serviceProvider.GetService<IQueueProcessor>();
        }

        public static IServiceCollection AddS3EmailMessageProcessor(this IServiceCollection services)
        {
            services.AddTransient<IPersistentEmailMessageInfoProcessor<AggregateReportInfo>> (_ => new PersistentEmailMessageInfoProcessor<AggregateReportInfo,AggregateReportEntity>(
                _.GetService<IEmailMessageInfoProcessor<AggregateReportInfo>>(),
                _.GetService<IToEntityConverter<AggregateReportInfo, AggregateReportEntity>>(),
                _.GetService<IReportDaoAsync<AggregateReportEntity>>(),
                _.GetService<ILogger>()));

            services.AddTransient<IPublishingEmailMessageInfoProcessor<AggregateReportInfo>>(_ => new AggregateReportPublishingEmailMessageInfoProcessor<AggregateReportInfo>(
                _.GetService<IPersistentEmailMessageInfoProcessor<AggregateReportInfo>>(),
                _.GetService<IEnumerable<IMessagePublisher>>(),
                _.GetService<ILogger>()));

            services.AddTransient<IS3EmailMessageProcessor>(_ => new S3EmailMessageProcessor<AggregateReportInfo>(
                _.GetService<IS3EmailMessageClient>(),
                _.GetService<IPublishingEmailMessageInfoProcessor<AggregateReportInfo>>(),
                _.GetService<ILambdaReportParserConfig>(),
                _.GetService<ILogger>()));

            return services;
        }
    }
}
