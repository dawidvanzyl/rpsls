using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Weighted.Abstracts;
using rpsls.Entities;
using rpsls.Entities.Enums;
using System.Collections.Immutable;

namespace rpsls.Domain.Algorithms.Weighted;

public class PercentileWeighting(ILogger<PercentileWeighting> logger)
    : AbstractWeightedAlgorithm(logger)
{
    protected override decimal CalculateWeight(IGrouping<AttackTypes, Match> attackGroup, IImmutableList<Match> matchHistory)
    {
        var allConsecutiveRepeats = matchHistory
            .Select(pa => pa.ConsecutiveRepeats)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var percentileRanks = attackGroup.Select(pa => (allConsecutiveRepeats.IndexOf(pa.ConsecutiveRepeats) + 1) / (decimal)allConsecutiveRepeats.Count);

        var averagePercentileRank = percentileRanks.Any()
            ? percentileRanks.Average()
            : 0m;

        var adjustedCount = attackGroup.Count() * averagePercentileRank;

        // Calculate the weighted percentage
        var weightedPercentage = adjustedCount / matchHistory.Count;

        logger.LogTrace("Average Percentile Rank: {AveragePercentileRank}", averagePercentileRank);
        logger.LogTrace("Adjusted Count: {AdjustedCount}", adjustedCount);
        logger.LogTrace("Weighted Percentage: {WeightedPercentage}", weightedPercentage);

        return weightedPercentage;
    }
}