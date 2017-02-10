using System.IO;
using System.IO.Compression;

namespace Dmarc.AggregateReport.Parser.Common.Compression
{
    public interface IGZipDecompressor : IDecompressor { }

    public class GZipDecompressor : IGZipDecompressor
    {
        public Stream Decompress(Stream stream)
        {
            using (GZipStream gzipStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                MemoryStream decompressedMemoryStream = new MemoryStream();
                gzipStream.CopyTo(decompressedMemoryStream);
                return decompressedMemoryStream;
            }
        }
    }
}