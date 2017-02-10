using System;

namespace Dmarc.Common.Environment
{
    public interface IEnvironmentVariables
    {
        string Get(string variableName);
        double GetAsDouble(string variableName);
        long GetAsLong(string variableName);
    }

    public class EnvironmentVariables : IEnvironmentVariables
    {
        private readonly IEnvironment _environment;

        public EnvironmentVariables(IEnvironment environment)
        {
            _environment = environment;
        }

        public string Get(string variableName)
        {
            string variable = _environment.GetEnvironmentalVariable(variableName);
            if (!string.IsNullOrWhiteSpace(variable))
            {
                return variable;
            }
            throw new ArgumentException($"No evironment variable exists with the name {variable}");
        }

        public double GetAsDouble(string variableName)
        {
            string variable = Get(variableName);
            double value;
            if (double.TryParse(variable, out value))
            {
                return value;
            }
            throw new ArgumentException($"{variableName} with value {variable} is not a valid double");
        }

        public long GetAsLong(string variableName)
        {
            string variable = Get(variableName);
            long value;
            if (long.TryParse(variable, out value))
            {
                return value;
            }
            throw new ArgumentException($"{variableName} with value {variable} is not a valid double");
        }
    }
}
