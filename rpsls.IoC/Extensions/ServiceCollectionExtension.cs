using Microsoft.Extensions.DependencyInjection;
using rpsls.Application;
using rpsls.Domain.Algorithms;
using rpsls.Domain.Modules;
using rpsls.Infrastructure.Repositories;

namespace rpsls.IoC.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection Application(this IServiceCollection services)
    {
        services.AddTransient<IGameService, GameService>();

        return services;
    }

    public static IServiceCollection Domain(this IServiceCollection services)
    {
        services.AddSingleton<IAlgorithm, SmoothedCountAlgorithm>();

        services
            .AddSingleton<IMatchResultModule, MatchResultModule>()
            .AddSingleton<IRuleModule, RuleModule>();

        return services;
    }

    public static IServiceCollection Infrastructure(this IServiceCollection services)
    {
        services
            .AddSingleton<IMatchResultRepository, MatchResultRepository>()
            .AddSingleton<IRuleRepository, RuleRepository>();

        return services;
    }
}