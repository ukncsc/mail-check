using System;
using System.IO;
using System.Linq;
using Dmarc.AggregateReport.Parser.Common.Compression;
using Dmarc.AggregateReport.Parser.Common.Domain;
using Dmarc.Common.Logging;
using MimeKit;

namespace Dmarc.AggregateReport.Parser.Common.Parsers
{
    public interface IAttachmentStreamNormaliser
    {
        AttachmentInfo Normalise(MimePart mimePart);
    }

    public class AttachmentStreamNormaliser : IAttachmentStreamNormaliser
    {
        private readonly IGZipDecompressor _gZipDecompressor;
        private readonly IZipDecompressor _zipDecompressor;
        private readonly ILogger _log;

        public AttachmentStreamNormaliser(IGZipDecompressor gZipDecompressor,
            IZipDecompressor zipDecompressor, ILogger log)
        {
            _gZipDecompressor = gZipDecompressor;
            _zipDecompressor = zipDecompressor;
            _log = log;
        }

        public AttachmentInfo Normalise(MimePart mimePart)
        {
            string contentType = mimePart.ContentType.MimeType.ToLower();
            switch (contentType)
            {
                case ContentType.ApplicationGzip:
                    return new AttachmentInfo(new AttachmentMetadata(mimePart.FileName), _gZipDecompressor.Decompress(GetDecodedStream(mimePart)));
                case ContentType.ApplicationZip:
                    return new AttachmentInfo(new AttachmentMetadata(mimePart.FileName), _zipDecompressor.Decompress(GetDecodedStream(mimePart)));
                case ContentType.ApplicationOctetStream:
                    string extension = Path.GetExtension(mimePart.FileName.Split('!').LastOrDefault() ?? string.Empty);
                    if (extension == ".gz")
                    {
                        return new AttachmentInfo(new AttachmentMetadata(mimePart.FileName), _gZipDecompressor.Decompress(GetDecodedStream(mimePart)));
                    }
                    if (extension == ".zip")
                    {
                        return new AttachmentInfo(new AttachmentMetadata(mimePart.FileName), _zipDecompressor.Decompress(GetDecodedStream(mimePart)));
                    }
                    if (extension == string.Empty)
                    {
                        _log.Info($"No extension for {mimePart.FileName}, trying to decompress as gzip.");
                        try
                        {
                            return new AttachmentInfo(new AttachmentMetadata(mimePart.FileName), _gZipDecompressor.Decompress(GetDecodedStream(mimePart)));
                        }
                        catch (Exception)
                        {
                            _log.Info($"Failed to decompress {mimePart.FileName} as gzip trying to decode as zip.");
                            try
                            {
                                return new AttachmentInfo(new AttachmentMetadata(mimePart.FileName), _zipDecompressor.Decompress(GetDecodedStream(mimePart)));
                            }
                            catch (Exception)
                            {
                                _log.Info($"Ignoring {mimePart.FileName} as failed to decompress as gzip or zip.");
                            }
                        }
                    }

                    _log.Warn($"Ignoring {mimePart.FileName}, unknown file type {(extension)}.");
                    return AttachmentInfo.EmptyAttachmentInfo;
                case ContentType.ApplicationXZipCompressed:
                    return new AttachmentInfo(new AttachmentMetadata(mimePart.FileName), _zipDecompressor.Decompress(GetDecodedStream(mimePart)));
                case ContentType.TextXml:
                    return new AttachmentInfo(new AttachmentMetadata(mimePart.FileName), GetDecodedStream(mimePart));
                default:
                    return AttachmentInfo.EmptyAttachmentInfo;
            }
        }

        private Stream GetDecodedStream(MimePart mimePart)
        {
            MemoryStream memoryStream = new MemoryStream();
            mimePart.ContentObject.DecodeTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}