using System.IO;

namespace Dmarc.AggregateReport.Parser.Common.Compression
{
    public interface IDecompressor
    {
        Stream Decompress(Stream stream);
    }
}