using Microsoft.Extensions.Logging;
using rpsls.Entities;
using rpsls.Entities.Enums;
using System.Collections.Immutable;

namespace rpsls.Domain.Algorithms.Weighted.Abstracts;

public abstract class AbstractWeightedAlgorithm(ILogger<AbstractWeightedAlgorithm> logger)
    : IWeightingAlgorithm
{
    public IImmutableDictionary<AttackTypes, decimal> CalculateWeights(IImmutableList<Match> matchHistory)
    {
        logger.LogDebug("Calculate weighed percentage");

        IImmutableDictionary<AttackTypes, decimal> attackPercentages = matchHistory
            .GroupBy((matchResult) => matchResult.P1Attack)
            .Select(attackGroup =>
            {
                logger.LogTrace("Attack: {Attack}", attackGroup.Key);
                logger.LogTrace("Count: {Count}", attackGroup.Count());

                return new
                {
                    Attack = attackGroup.Key,
                    Weight = CalculateWeight(attackGroup, matchHistory)
                };
            })
            .ToImmutableDictionary(
                a => a.Attack,
                a => a.Weight);

        return attackPercentages;
    }

    protected abstract decimal CalculateWeight(IGrouping<AttackTypes, Match> attackGroup, IImmutableList<Match> matchHistory);
}