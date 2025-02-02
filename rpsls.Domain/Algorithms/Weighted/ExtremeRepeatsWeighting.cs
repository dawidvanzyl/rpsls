using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Weighted.Abstracts;
using rpsls.Entities;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.Weighted;

public class ExtremeRepeatsWeighting(ILogger<ExtremeRepeatsWeighting> logger)
    : AbstractWeightedAlgorithm(logger)
{
    protected override decimal CalculateWeight(IGrouping<AttackTypes, Round> attackGroup, IList<Round> history)
    {
        var scalingFactor = -0.1d;
        var adjustmentFactor = 5;

        var maxConsecutiveRepeats = attackGroup.Max(match => match.ConsecutiveRepeats);
        var sigmoid = 1 / (1 + Math.Exp(scalingFactor * (maxConsecutiveRepeats - adjustmentFactor))); // Adjusting steepness
        var attackFrequency = history.Count == 0
            ? 0m
            : (decimal)attackGroup.Count() / history.Count;

        // Calculate the weighted percentage
        var weightedPercentage = attackFrequency * (decimal)sigmoid;

        logger.LogTrace("Sigmoid: {Sigmoid}", sigmoid);
        logger.LogTrace("Attack Frequency: {AttackFrequency}", attackFrequency);
        logger.LogTrace("Weighted Percentage: {WeightedPercentage}", weightedPercentage);

        return weightedPercentage;
    }
}