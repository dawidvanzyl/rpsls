using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Weighted.Abstracts;
using rpsls.Entities;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.Weighted;

public class FrequencyWeighting(ILogger<FrequencyWeighting> logger)
    : AbstractWeightedAlgorithm(logger)
{
    protected override decimal CalculateWeight(IGrouping<AttackTypes, Round> attackGroup, IList<Round> history)
    {
        var attackFrequency = history.Count == 0
            ? 0m
            : (decimal)attackGroup.Count() / history.Count; // Relative frequency of attack

        var consecutiveRepeatFrequency = (decimal)attackGroup.Count() / attackGroup.Aggregate(0L, (current, next) => next.ConsecutiveRepeats + current);

        // Combine with normalization
        var weightedPercentage = attackFrequency * consecutiveRepeatFrequency;

        logger.LogTrace("Attack Frequency: {AttackFrequency}", attackFrequency);
        logger.LogTrace("Weighted Percentage: {WeightedPercentage}", weightedPercentage);

        return weightedPercentage;
    }
}