using System.IO;

namespace Dmarc.AggregateReport.Parser.Lambda.Compression
{
    public interface IDecompressor
    {
        string StreamType { get; }
        Stream Decompress(Stream stream);
    }
}