using System;
using Dmarc.AggregateReport.Parser.Lambda.Compression;
using Dmarc.AggregateReport.Parser.Lambda.Converters;
using Dmarc.AggregateReport.Parser.Lambda.Domain;
using Dmarc.AggregateReport.Parser.Lambda.Parser;
using Dmarc.AggregateReport.Parser.Lambda.Persistence.FileSystem;
using Dmarc.AggregateReport.Parser.Lambda.Persistence.Multi;
using Dmarc.AggregateReport.Parser.Lambda.Persistence.Single;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation;
using Dmarc.AggregateReport.Parser.Lambda.Serialisation.AggregateReportDeserialisation;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Report.Email;
using Dmarc.Common.Report.File;
using Dmarc.Common.Report.Parser;
using Dmarc.Common.Report.Persistance;
using Dmarc.Common.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Dmarc.AggregateReport.Parser.Lambda.Factory
{
    internal static class AggregateReportParserAppFactory
    {
        public static IFileEmailMessageProcessor Create(CommandLineArgs commandLineArgs, ILogger log)
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                .AddAttachmentPersistors(commandLineArgs)
                .AddDenormalisedRecordPersistors(commandLineArgs)
                .AddSingleton(log)
                .AddTransient<IMimeMessageFactory, MimeMessageFactory>()
                .AddTransient<IGZipDecompressor, GZipDecompressor>()
                .AddTransient<IZipDecompressor, ZipDecompressor>()
                .AddTransient<IDenormalisedRecordConverter, DenormalisedRecordConverter>()
                .AddTransient<IAggregateReportDeserialiser, AggregateReportDeserialiser>()
                .AddTransient<IAttachmentStreamNormaliser, AttachmentStreamNormaliser>()
                .AddTransient<IContentTypeProvider, ContentTypeProvider>()
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
                .AddTransient<IReportParser<AggregateReportInfo>, AggregateReportParserVerbose>()
                .AddTransient<IFileEmailMessageProcessor, FileEmailMessageProcessor<AggregateReportInfo>>()
                .AddTransient<ICsvDenormalisedRecordSerialiser, CsvDenormalisedRecordSerialiser>()
                .AddTransient<IReportPersistor<AggregateReportInfo>, MultiAggregateReportPersistor>()
                .BuildServiceProvider();

            return serviceProvider.GetService<IFileEmailMessageProcessor>();
        }

        private static IServiceCollection AddAttachmentPersistors(this IServiceCollection serviceCollection, CommandLineArgs commandLineArgs)
        {
            serviceCollection.AddSingleton<IMultiAttachmentPersistor, MultiAttachmentPersistor>();

            if (commandLineArgs.XmlDirectory != null)
            {
                serviceCollection.AddTransient<IAttachmentPersitor>(provider =>
                    new XmlAttachmentPersitor(commandLineArgs.XmlDirectory));
            }

            return serviceCollection;
        }

        private static IServiceCollection AddDenormalisedRecordPersistors(this IServiceCollection serviceCollection, CommandLineArgs commandLineArgs)
        {
            serviceCollection.AddSingleton<IMultiDenormalisedRecordPersistor, MulitDenormalisedRecordPersistor>();

            if (commandLineArgs.CsvFile != null)
            {
                serviceCollection.AddTransient<IDenormalisedRecordPersistor>(
                    provider => new CsvDenormalisedRecordPersistor(commandLineArgs.CsvFile,
                    provider.GetService<ICsvDenormalisedRecordSerialiser>()));
            }

            if (commandLineArgs.SqlFile != null)
            {
                serviceCollection.AddTransient<IDenormalisedRecordPersistor>(
                    provider => new SqliteDenormalisedRecordPersistor(commandLineArgs.SqlFile));
            }

            if (commandLineArgs.CsvFile == null && commandLineArgs.SqlFile == null)
            {
                serviceCollection.AddTransient<IDenormalisedRecordPersistor, Persistence.Console.CsvDenormalisedRecordPersistor>();
            }

            return serviceCollection;
        }
    }
}
