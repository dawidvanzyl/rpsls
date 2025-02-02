using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using rpsls.Application;
using rpsls.Domain.Algorithms.Modifiers;
using rpsls.Domain.Algorithms.RecencyBiases;
using rpsls.Domain.Algorithms.Weighted;
using rpsls.Domain.Modules;
using rpsls.Infrastructure.Repositories;
using rpsls.IoC.Options;

namespace rpsls.IoC.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection Application(this IServiceCollection services)
    {
        services
            .AddTransient<IGameService, GameService>();

        return services;
    }

    public static IServiceCollection Domain(this IServiceCollection services)
    {
        services
            .AddKeyedSingleton<IWeightingAlgorithm, ExponentialSmoothing>(nameof(ExponentialSmoothing))
            .AddKeyedSingleton<IWeightingAlgorithm, LogarithmicWeighting>(nameof(LogarithmicWeighting))
            .AddKeyedSingleton<IRecencyBiasAlgorithm, WeightedRecencyBias>(nameof(WeightedRecencyBias))
            .AddKeyedSingleton<IRecencyBiasAlgorithm, RecencyBiasByMatchCount>(nameof(RecencyBiasByMatchCount))
            .AddKeyedSingleton<IRecencyBiasAlgorithm, ExponentialDecayRecencyBias>(nameof(ExponentialDecayRecencyBias))
            .AddKeyedSingleton<IRecencyBiasAlgorithm, NormalizedRecencyBias>(nameof(NormalizedRecencyBias));

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AlgorithmOptions>>().Value;
            return sp.GetKeyedService<IWeightingAlgorithm>(options.Modifier);
        });

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AlgorithmOptions>>().Value;
            return sp.GetKeyedService<IRecencyBiasAlgorithm>(options.RecencyBias);
        });

        services
            .AddSingleton<IGameModule, GameModule>()
            .AddSingleton<IRuleModule, RuleModule>();

        services.AddSingleton<IAttackPredictor, AttackPredictor>();

        return services;
    }

    public static IServiceCollection Infrastructure(this IServiceCollection services)
    {
        services
            .AddSingleton<IGameRepository, GameRepository>()
            .AddSingleton<IRuleRepository, RuleRepository>();

        return services;
    }
}