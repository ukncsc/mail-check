namespace Dmarc.ForensicReport.Parser.Lambda.Dao.Entities
{
    public class HostnameEntity
    {
        public HostnameEntity(string name)
        {
            Name = name;
        }

        public long Id { get; set; }

        public string Name { get; }

        //public IpAddressEntity IpAddress { get; set; }
    }
}