using Microsoft.Extensions.Logging;
using rpsls.Entities;
using rpsls.Entities.Enums;
using System.Collections.Immutable;

namespace rpsls.Domain.Algorithms.RecencyBiases.Abstracts;

public abstract class AbstactRecencyBiasAlgorithm(ILogger<AbstactRecencyBiasAlgorithm> logger)
    : IRecencyBiasAlgorithm
{
    public IImmutableDictionary<AttackTypes, decimal> ApplyRecencyBias(IImmutableDictionary<AttackTypes, decimal> attackPercentages, IImmutableList<Match> matchHistory)
    {
        logger.LogDebug("Apply recency bais");

        var recencyBiasFactor = matchHistory[^1].Result switch
        {
            ResultTypes.Win => 0.3m,
            ResultTypes.Draw => 0.15m,
            _ => -0.075m
        };

        var boostedAttackPercentages = attackPercentages
            .ToImmutableDictionary(
                kv => kv.Key,
                kv =>
                {
                    if (matchHistory[^1].P1Attack == kv.Key)
                    {
                        logger.LogTrace("Attack: {Attack}", kv.Key);

                        var boostedPercentage = GetBoostedPercentage(kv.Key, kv.Value, recencyBiasFactor, matchHistory);

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
        IImmutableList<Match> matchHistory);
}