using Microsoft.Extensions.Logging;
using rpsls.Entities;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.Weighted.Abstracts;

public abstract class AbstractWeightedAlgorithm(ILogger<AbstractWeightedAlgorithm> logger)
    : IWeightingAlgorithm
{
    public IDictionary<AttackTypes, decimal> CalculateWeights(IList<Round> history)
    {
        logger.LogDebug("Calculate weighed percentage");

        IDictionary<AttackTypes, decimal> attackPercentages = history
            .GroupBy((matchResult) => matchResult.P1Attack)
            .Select(attackGroup =>
            {
                logger.LogTrace("Attack: {Attack}", attackGroup.Key);
                logger.LogTrace("Count: {Count}", attackGroup.Count());

                return new
                {
                    Attack = attackGroup.Key,
                    Weight = CalculateWeight(attackGroup, history)
                };
            })
            .ToDictionary(
                a => a.Attack,
                a => a.Weight);

        return attackPercentages;
    }

    protected abstract decimal CalculateWeight(IGrouping<AttackTypes, Round> attackGroup, IList<Round> history);
}