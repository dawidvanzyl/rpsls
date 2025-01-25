using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Weighted.Abstracts;
using rpsls.Entities;
using rpsls.Entities.Enums;
using System.Collections.Immutable;

namespace rpsls.Domain.Algorithms.Weighted;

public class ExponentialSmoothing(ILogger<ExponentialSmoothing> logger)
    : AbstractWeightedAlgorithm(logger)
{
    protected override decimal CalculateWeight(IGrouping<AttackTypes, Match> attackGroup, IImmutableList<Match> matchHistory)
    {
        if (matchHistory.Count == 0)
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
        var weightedPercentage = smoothedCount / matchHistory.Count;

        logger.LogTrace("Smoothed Count: {SmoothedCount}", smoothedCount);
        logger.LogTrace("Weighted Percentage: {WeightedPercentage}", weightedPercentage);

        return weightedPercentage;
    }
}