using System.Threading.Tasks;
using Dmarc.MxSecurityTester.Util;

namespace Dmarc.MxSecurityTester.Smtp
{
    internal interface ISmtpSerializer
    {
        Task Serialize(Command command, IStreamWriter streamWriter);
    }

    internal class SmtpSerializer : ISmtpSerializer
    {
        public Task Serialize(Command command, IStreamWriter streamWriter)
        {
            return streamWriter.WriteLineAsync(command.CommandString);
        }
    }
}