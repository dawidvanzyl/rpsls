using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Weighted.Abstracts;
using rpsls.Entities;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.Weighted;

public class DynamicThresholdWeighting(ILogger<DynamicThresholdWeighting> logger)
    : AbstractWeightedAlgorithm(logger)
{
    protected override decimal CalculateWeight(IGrouping<AttackTypes, Round> attackGroup, IList<Round> history)
    {
        var threshold = 5; // Arbitrary threshold for repetition

        // Use the max consecutive repeats instead of the sum
        var maxConsecutiveRepeats = attackGroup.Max(match => match.ConsecutiveRepeats);
        var attackFrequency = (decimal)attackGroup.Count() / history.Count(); // Relative frequency

        // Apply modifier based on the threshold
        var modifier = maxConsecutiveRepeats > threshold
            ? attackGroup.Average(match => 1m / match.ConsecutiveRepeats) // Scale down for high repetition
            : 1m; // No modifier for small repetition counts

        // Calculate the weighted percentage
        var weightedPercentage = attackFrequency * modifier;

        logger.LogTrace("Modifier: {Modifier}", modifier);
        logger.LogTrace("Weighted Percentage: {WeightedPercentage}", weightedPercentage);

        return weightedPercentage;
    }
}