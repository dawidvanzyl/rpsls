using Microsoft.Extensions.DependencyInjection;

namespace rpsls.Infrastructure
{
    public static class ServiceCollectionFactory
    {
        public static IServiceCollection Create()
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton<IConfigurationFactory, ConfigurationFactory>()
                .AddSingleton<AppSettings>();

            return serviceCollection;
        }
    }
}
