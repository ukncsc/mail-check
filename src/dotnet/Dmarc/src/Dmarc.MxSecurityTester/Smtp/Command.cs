namespace Dmarc.MxSecurityTester.Smtp
{
    public abstract class Command
    {
        protected readonly string CommandBase;

        protected Command(string commandBase)
        {
            CommandBase = commandBase;
            CommandString = commandBase;
        }

        public string CommandString { get; protected set; }
    }
}