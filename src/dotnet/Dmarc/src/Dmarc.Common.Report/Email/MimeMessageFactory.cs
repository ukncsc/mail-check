using System.IO;
using MimeKit;

namespace Dmarc.Common.Report.Email
{
    public interface IMimeMessageFactory
    {
        MimeMessage Create(Stream stream);
    }

    public class MimeMessageFactory : IMimeMessageFactory
    {
        public MimeMessage Create(Stream stream)
        {
            MimeParser parser = new MimeParser(stream, MimeFormat.Entity);
            return parser.ParseMessage();
        }
    }
}
