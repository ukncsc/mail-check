using System.IO;
using System.IO.Compression;

namespace Dmarc.AggregateReport.Parser.Lambda.Compression
{
    public interface IGZipDecompressor : IDecompressor { }

    public class GZipDecompressor : IGZipDecompressor
    {
        public string StreamType => "GZip";

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