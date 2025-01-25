using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Weighted.Abstracts;
using rpsls.Entities;
using rpsls.Entities.Enums;
using System.Collections.Immutable;

namespace rpsls.Domain.Algorithms.Weighted;

public class FrequencyWeighting(ILogger<FrequencyWeighting> logger)
    : AbstractWeightedAlgorithm(logger)
{
    protected override decimal CalculateWeight(IGrouping<AttackTypes, Match> attackGroup, IImmutableList<Match> matchHistory)
    {
        var attackFrequency = matchHistory.Count == 0
            ? 0m
            : (decimal)attackGroup.Count() / matchHistory.Count; // Relative frequency of attack

        var consecutiveRepeatFrequency = (decimal)attackGroup.Count() / attackGroup.Aggregate(0L, (current, next) => next.ConsecutiveRepeats + current);

        // Combine with normalization
        var weightedPercentage = attackFrequency * consecutiveRepeatFrequency;

        logger.LogTrace("Attack Frequency: {AttackFrequency}", attackFrequency);
        logger.LogTrace("Weighted Percentage: {WeightedPercentage}", weightedPercentage);

        return weightedPercentage;
    }
}