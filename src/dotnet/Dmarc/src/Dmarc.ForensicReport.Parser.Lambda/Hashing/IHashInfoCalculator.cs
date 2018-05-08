using Dmarc.ForensicReport.Parser.Lambda.Domain;
using MimeKit;

namespace Dmarc.ForensicReport.Parser.Lambda.Hashing
{
    public interface IHashInfoCalculator
    {
        HashInfo Calculate(MimePart mimePart);
    }
}