using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Contexts;
using rpsls.Entities.Enums;
using System.Collections.Immutable;

namespace rpsls.Domain.Algorithms.RecencyBiases.Abstracts;

public abstract class AbstactRecencyBias(ILogger<AbstactRecencyBias> logger)
    : IRecencyBiasAlgorithm
{
    public IDictionary<AttackTypes, decimal> ApplyRecencyBias(
        IImmutableDictionary<AttackTypes, decimal> attackPercentages,
        AlgorithmContext context)
    {
        var recencyBiasFactor = context.LastMatch.Result switch
        {
            ResultTypes.Win => 0.3m,
            ResultTypes.Draw => 0.15m,
            _ => -0.075m
        };

        var boostedAttackPercentages = attackPercentages
            .ToDictionary(
                kv => kv.Key,
                kv =>
                {
                    if (context.LastMatch.P1Attack == kv.Key)
                    {
                        logger.LogDebug("Attack: {Attack}", kv.Key);

                        var boostedPercentage = GetBoostedPercentage(kv.Key, kv.Value, recencyBiasFactor, context);

                        return boostedPercentage;
                    }
                    else
                    {
                        return kv.Value;
                    }
                });

        return boostedAttackPercentages;
    }

    protected abstract decimal GetBoostedPercentage(
        AttackTypes attack,
        decimal percentage,
        decimal recencyBiasFactor,
        AlgorithmContext context);
}