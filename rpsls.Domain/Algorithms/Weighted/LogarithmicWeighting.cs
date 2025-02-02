using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Weighted.Abstracts;
using rpsls.Entities;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.Modifiers;

public class LogarithmicWeighting(ILogger<LogarithmicWeighting> logger)
    : AbstractWeightedAlgorithm(logger)
{
    protected override decimal CalculateWeight(IGrouping<AttackTypes, Round> attackGroup, IList<Round> history)
    {
        var logarithmicSum = attackGroup
            .Select(match => Math.Log(match.ConsecutiveRepeats + 1)) // Apply logarithm to each value
            .Sum();

        if (logarithmicSum == 0)
        {
            return 0m; // Avoid division by zero
        }

        // Calculate the weighted percentage
        var weightedPercentage = logarithmicSum / history.Count; // Normalize against total count

        logger.LogTrace("Logarithmic Sum: {LogarithmicSum}", logarithmicSum);
        logger.LogTrace("Weighted Percentage: {WeightedPercentage}", weightedPercentage);

        return (decimal)weightedPercentage;
    }
}