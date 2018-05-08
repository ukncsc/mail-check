using System;
using System.Net;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;

namespace Dmarc.ForensicReport.Parser.Lambda.Converters
{
    public interface IIpAddressToEntityConverter
    {
        IpAddressEntity Convert(IPAddress ipAddress);
    }

    public class IpAddressToEntityConverter : IIpAddressToEntityConverter
    {
        public IpAddressEntity Convert(IPAddress ipAddress)
        {
            string binaryIpAddress = $"0x{BitConverter.ToString(ipAddress.GetAddressBytes()).Replace("-", string.Empty)}";
            return new IpAddressEntity(ipAddress.ToString(), binaryIpAddress);
        }
    }
}
