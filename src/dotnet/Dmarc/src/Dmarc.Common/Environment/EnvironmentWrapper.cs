namespace Dmarc.Common.Environment
{
    public interface IEnvironment
    {
        string GetEnvironmentalVariable(string variableName);
    }

    public class EnvironmentWrapper : IEnvironment
    {
        public string GetEnvironmentalVariable(string variableName)
        {
            return System.Environment.GetEnvironmentVariable(variableName);
        }
    }
}