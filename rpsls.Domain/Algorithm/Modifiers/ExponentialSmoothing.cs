using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Contexts;
using rpsls.Domain.Algorithms.Modifiers.Abstracts;
using rpsls.Entities;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.Modifiers;

public class ExponentialSmoothing(ILogger<ExponentialSmoothing> logger)
    : AbstractModifier(logger)
{
    protected override decimal CalculateWeightedPercentage(IGrouping<AttackTypes, Match> attackGroup, AlgorithmContext context)
    {
        if (context.TotalCount == 0)
        {
            return 0m; // Avoid division by zero
        }

        var smoothedCount = 1m;
        var alpha = 0.1m; // Smoothing factor

        // Apply exponential smoothing over the range of consecutive repeats
        for (var consecutiveRepeats = 1; consecutiveRepeats <= attackGroup.Count(); consecutiveRepeats++)
        {
            smoothedCount = (alpha * consecutiveRepeats) + ((1 - alpha) * smoothedCount);
        }

        // Calculate the weighted percentage
        var weightedPercentage = smoothedCount / context.TotalCount;

        logger.LogTrace("Smoothed Count: {SmoothedCount}", smoothedCount);
        logger.LogTrace("Weighted Percentage: {WeightedPercentage}", weightedPercentage);

        return weightedPercentage;
    }
}