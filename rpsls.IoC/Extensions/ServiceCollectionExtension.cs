using Microsoft.Extensions.DependencyInjection;
using rpsls.Application;
using rpsls.Domain.Algorithms.Models;
using rpsls.Domain.Algorithms.RecencyBias;
using rpsls.Domain.Algorithms.Weight;
using rpsls.Domain.Modules;
using rpsls.Infrastructure.Repositories;

namespace rpsls.IoC.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection Application(this IServiceCollection services)
    {
        services
            .AddTransient<IGameService, GameService>()
            .AddTransient<IAttackService, AttackService>();

        return services;
    }

    public static IServiceCollection Domain(this IServiceCollection services)
    {
        services
            .AddSingleton<IWeightAlgorithm<WeightedSmoothedCountInput>, WeightedSmoothedCount>()
            .AddSingleton<IRecencyBiasAlgorithm<WeightedRecencyBiasInput>, WeightedRecencyBias>();

        services
            .AddSingleton<IMatchModule, MatchModule>()
            .AddSingleton<IRuleModule, RuleModule>();

        return services;
    }

    public static IServiceCollection Infrastructure(this IServiceCollection services)
    {
        services
            .AddSingleton<IMatchRepository, MatchRepository>()
            .AddSingleton<IRuleRepository, RuleRepository>();

        return services;
    }
}