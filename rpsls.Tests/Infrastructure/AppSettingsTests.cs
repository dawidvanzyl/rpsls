using Microsoft.Extensions.Configuration;
using rpsls.Infrastructure.Factories;
using rpsls.Infrastructure.Repositories;
using System.IO;

namespace rpsls.Tests.Infrastructure
{
    public partial class AppSettingsTests
    {
        private AppSettings Make_AppSettings(string configFile)
        {
            var configurationFactory = new AppSettingsTestsConfigurationFactory(configFile);
            return new AppSettings(configurationFactory);
        }
    }

    public sealed class AppSettingsTestsConfigurationFactory : IConfigurationFactory
    {
        private readonly string configFile;

        public AppSettingsTestsConfigurationFactory(string configFile)
        {
            this.configFile = configFile;
        }

        public IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile(configFile, optional: true, reloadOnChange: true);

            return builder.Build();
        }
    }
}