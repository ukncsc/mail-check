namespace Dmarc.AggregateReport.Api.Dao.Entities
{
    internal class Domain
    {
        public Domain(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public string Name { get; }
        public int Id { get; }
    }
}
