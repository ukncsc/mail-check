using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Dmarc.Metrics.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseKestrel()
                .UseHealthChecks("/healthcheck")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<StartUp>()
                .Build()
                .Run();
        }
    }
}
