using System.IO;
using MimeKit;

namespace Dmarc.AggregateReport.Parser.Lambda.Utils
{
    public static class MimePartExtensionMethods
    {
        public static Stream GetDecodedStream(this MimePart mimePart)
        {
            MemoryStream memoryStream = new MemoryStream();
            mimePart.Content.DecodeTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }
    }
}