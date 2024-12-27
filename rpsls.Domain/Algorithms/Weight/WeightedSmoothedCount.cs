using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Models;

namespace rpsls.Domain.Algorithms.Weight;

public class WeightedSmoothedCount(ILogger<WeightedSmoothedCount> logger)
    : IWeightAlgorithm<WeightedSmoothedCountInput>
{
    public decimal CalculateWeightedPercentage(WeightedSmoothedCountInput input)
    {
        if (input.TotalCount == 0)
        {
            return 0m; // Avoid division by zero
        }

        var smoothedCount = 1m;
        var alpha = 0.1m; // Smoothing factor

        // Apply exponential smoothing over the range of consecutive repeats
        for (var consecutiveRepeats = 1; consecutiveRepeats <= input.GroupCount; consecutiveRepeats++)
        {
            smoothedCount = (alpha * consecutiveRepeats) + ((1 - alpha) * smoothedCount);
        }

        // Calculate the weighted percentage
        var weightedPercentage = smoothedCount / input.TotalCount;

        logger.LogDebug("Smoothed Count: {SmoothedCount}", smoothedCount);
        logger.LogDebug("Weighted Percentage: {WeightedPercentage}", weightedPercentage);

        return weightedPercentage;
    }
}