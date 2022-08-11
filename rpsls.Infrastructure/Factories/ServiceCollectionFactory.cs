using Microsoft.Extensions.DependencyInjection;
using rpsls.Application;
using rpsls.Application.Repositories;
using rpsls.Infrastructure.Algorithms;

namespace rpsls.Infrastructure.Factories
{
    public static class ServiceCollectionFactory
    {
        public static IServiceCollection Create()
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton<IAttackStrategyFactory, AttackAlgorithmFactory>()
                .AddSingleton<IConfigurationFactory, ConfigurationFactory>()
                .AddSingleton<ISettingsRepository, AppSettings>();

            return serviceCollection;
        }
    }
}