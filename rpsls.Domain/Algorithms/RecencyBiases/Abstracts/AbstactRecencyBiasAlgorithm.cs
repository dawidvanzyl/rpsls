using Microsoft.Extensions.Logging;
using rpsls.Domain.Modules;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.RecencyBiases.Abstracts;

public abstract class AbstactRecencyBiasAlgorithm(ILogger<AbstactRecencyBiasAlgorithm> logger, IGameModule gameModule)
    : IRecencyBiasAlgorithm
{
    public IGameModule GameModule { get; } = gameModule;

    public IDictionary<AttackTypes, decimal> ApplyRecencyBias(IDictionary<AttackTypes, decimal> attackPercentages)
    {
        if (GameModule.Current.Rounds.Count == 0)
        {
            return attackPercentages;
        }

        logger.LogDebug("Apply recency bais");

        var recencyBiasFactor = GameModule.Current.Rounds[^1].Result switch
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
                    if (GameModule.Current.Rounds[^1].P1Attack == kv.Key)
                    {
                        logger.LogTrace("Attack: {Attack}", kv.Key);

                        var boostedPercentage = GetBoostedPercentage(kv.Key, kv.Value, recencyBiasFactor);

                        return boostedPercentage;
                    }
                    else
                    {
                        return kv.Value;
                    }
                });

        return boostedAttackPercentages;
    }

    protected abstract decimal GetBoostedPercentage(AttackTypes attack, decimal percentage, decimal recencyBiasFactor);
}