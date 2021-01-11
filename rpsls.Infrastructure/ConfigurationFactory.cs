using Microsoft.Extensions.Configuration;
using System.IO;

namespace rpsls.Infrastructure
{
    public interface IConfigurationFactory
    {
        IConfiguration GetConfiguration();
    }

    public sealed class ConfigurationFactory : IConfigurationFactory
    {
        public IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }
    }
}
