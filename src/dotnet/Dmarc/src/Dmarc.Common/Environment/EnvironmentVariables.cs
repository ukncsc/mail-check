using System;
using System.Net;

namespace Dmarc.Common.Environment
{
    public interface IEnvironmentVariables
    {
        string Get(string variableName, bool throwIfNotFound = true);
        bool GetAsBoolOrDefault(string variableName, bool defaultValue = false);
        int GetAsInt(string variableName);
        double GetAsDouble(string variableName);
        long GetAsLong(string variableName);
        IPAddress GetAsIpAddress(string variableName);
    }

    public class EnvironmentVariables : IEnvironmentVariables
    {
        private readonly IEnvironment _environment;

        public EnvironmentVariables(IEnvironment environment)
        {
            _environment = environment;
        }

        public string Get(string variableName, bool throwIfNotFound = true)
        {
            string variable = _environment.GetEnvironmentalVariable(variableName);

            if (!throwIfNotFound || !string.IsNullOrWhiteSpace(variable))
            {
                return variable;
            }
            
            throw new ArgumentException($"No environment variable exists with the name { variableName }");
        }

        public bool GetAsBoolOrDefault(string variableName, bool defaultValue = false)
        {
            string variable = Get(variableName, false);
            bool value;
            return bool.TryParse(variable, out value) ? value : defaultValue;
        }

        public int GetAsInt(string variableName)
        {
            string variable = Get(variableName);
            int value;
            if (int.TryParse(variable, out value))
            {
                return value;
            }
            throw new ArgumentException($"{variableName} with value {variable} is not a valid int.");
        }

        public double GetAsDouble(string variableName)
        {
            string variable = Get(variableName);
            double value;
            if (double.TryParse(variable, out value))
            {
                return value;
            }
            throw new ArgumentException($"{variableName} with value {variable} is not a valid double.");
        }

        public long GetAsLong(string variableName)
        {
            string variable = Get(variableName);
            long value;
            if (long.TryParse(variable, out value))
            {
                return value;
            }
            throw new ArgumentException($"{variableName} with value {variable} is not a valid long.");
        }

        public IPAddress GetAsIpAddress(string variableName)
        {
            string variable = Get(variableName);
            IPAddress address;
            if (IPAddress.TryParse(variable, out address))
            {
                return address;
            }
            throw new ArgumentException($"{variableName} with value {variable} is not a valid ip address.");
        }
    }
}
