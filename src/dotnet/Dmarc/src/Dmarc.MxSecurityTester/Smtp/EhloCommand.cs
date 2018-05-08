namespace Dmarc.MxSecurityTester.Smtp
{
    internal class EhloCommand : Command
    {
        public EhloCommand(string smtpHostName)
            : base("EHLO")
        {
            CommandString = $"{CommandBase} {smtpHostName}";
        }
    }
}