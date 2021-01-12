using Microsoft.Extensions.DependencyInjection;
using rpsls.Application;
using rpsls.Application.Repositories;
using rpsls.Infrastructure.Algorithms;
using rpsls.Infrastructure.Repositories;

namespace rpsls.Infrastructure.Factories
{
    public static class ServiceCollectionFactory
    {
        public static IServiceCollection Create()
        {
            var serviceCollection = new ServiceCollection()
                .AddSingleton<GameServiceFactory>()
                .AddSingleton<GameService>()
                .AddSingleton<IAttackStrategyFactory, AttackAlgorithmFactory>()
                .AddSingleton<IMatchRepository, MatchFileRepository>()
                .AddSingleton<IConfigurationFactory, ConfigurationFactory>()
                .AddSingleton<ISettingsRepository, AppSettings>();

            return serviceCollection;
        }
    }
}
