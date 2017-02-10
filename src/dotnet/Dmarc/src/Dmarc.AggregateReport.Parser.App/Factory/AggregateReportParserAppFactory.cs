using System;
using Dmarc.AggregateReport.Parser.App.Parsers;
using Dmarc.AggregateReport.Parser.Common.Compression;
using Dmarc.AggregateReport.Parser.Common.Converters;
using Dmarc.AggregateReport.Parser.Common.Email;
using Dmarc.AggregateReport.Parser.Common.Parsers;
using Dmarc.AggregateReport.Parser.Common.Persistence.FileSystem;
using Dmarc.AggregateReport.Parser.Common.Persistence.Multi;
using Dmarc.AggregateReport.Parser.Common.Persistence.Single;
using Dmarc.AggregateReport.Parser.Common.Serialisation;
using Dmarc.AggregateReport.Parser.Common.Serialisation.AggregateReportDeserialisation;
using Dmarc.Common.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Dmarc.AggregateReport.Parser.App.Factory
{
    internal static class AggregateReportParserAppFactory
    {
        public static IFileEmailMessageProcessor Create(CommandLineArgs commandLineArgs, ILogger log)
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                .AddAggregateReportPersistors(commandLineArgs)
                .AddDenormalisedRecordPersistors(commandLineArgs)
                .AddSingleton(log)
                .AddTransient<IMimeMessageFactory, MimeMessageFactory>()
                .AddTransient<IGZipDecompressor, GZipDecompressor>()
                .AddTransient<IZipDecompressor, ZipDecompressor>()
                .AddTransient<IDenormalisedRecordConverter, DenormalisedRecordConverter>()
                .AddTransient<IAggregateReportDeserialiser, AggregateReportDeserialiser>()
                .AddTransient<IAttachmentStreamNormaliser, AttachmentStreamNormaliser>()
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
                .AddTransient<IAggregateReportParser, AggregateReportParserApp>()
                .AddTransient<IFileEmailMessageProcessor, AggregateReportFileEmailMessageProcessor>()
                .AddTransient<ICsvDenormalisedRecordSerialiser, CsvDenormalisedRecordSerialiser>()
                .AddTransient<IMultiAggregateReportPersistor, MultiAggregateReportPersistor>()
                .BuildServiceProvider();

            return serviceProvider.GetService<IFileEmailMessageProcessor>();
        }

        private static IServiceCollection AddAggregateReportPersistors(this IServiceCollection serviceCollection, CommandLineArgs commandLineArgs)
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
