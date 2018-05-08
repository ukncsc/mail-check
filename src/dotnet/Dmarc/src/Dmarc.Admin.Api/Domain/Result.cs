namespace Dmarc.Admin.Api.Domain
{
    public class Result<T>
    {
        public Result(string name, T t)
        {
            Name = name;
            Results = t;
        }

        public string Name { get; }
        public T Results { get; }
    }
}