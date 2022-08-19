using Microsoft.Extensions.DependencyInjection;
using rpsls.Application;
using rpsls.Infrastructure.Algorithms;
using rpsls.Infrastructure.Factories;
using rpsls.Infrastructure.Repositories;

namespace rpsls.IoC.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDefaults(this IServiceCollection services)
        {
            services
                .AddSingleton<GameServiceFactory>()
                .AddSingleton<GameService>()
                .AddSingleton<IAlgorithmContextFactory, AlgorithmContextFactory>()
                .AddSingleton<IChallengeAlgorithmStrategy, ChallengeAlgorithmStrategy>()
                .AddTransient<MLChallengeAlgorithm>()
                .AddTransient<RandomChallengeAlgorithm>()
                .AddSingleton<IMatchRepository, MatchFileRepository>()
                .AddSingleton<IConfigurationFactory, ConfigurationFactory>()
                .AddSingleton<ISettingsRepository, AppSettings>();

            return services;
        }
    }
}