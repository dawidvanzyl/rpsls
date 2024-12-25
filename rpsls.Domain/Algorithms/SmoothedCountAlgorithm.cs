using Microsoft.Extensions.Logging;
using rpsls.Domain.Modules;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms;

public class SmoothedCountAlgorithm(
    IMatchResultModule matchResultModule,
    IRuleModule ruleSetModule,
    ILogger<SmoothedCountAlgorithm> logger)
    : IAlgorithm
{
    public AttackTypes CalculateAttack()
    {
        var matchResults = matchResultModule.GetAll();

        if (!matchResults.Any())
        {
            return (AttackTypes)Random.Shared.Next(1, 3);
        }

        var attackPercentages = matchResults
            .GroupBy((matchResult) => matchResult.P1Attack)
            .Select(attackGroup =>
            {
                var totalCount = matchResults.Count;
                var count = attackGroup.Count();

                var smoothedCount = CalculateSmoothedCount(attackGroup.Count());
                var weighedPercentage = CalculateWeightedPercentage(smoothedCount, totalCount);

                logger.LogDebug(
                    "Attack: {Attack}, Count: {Count}, Smoothed Count: {SmoothedCount}, Weighted Percentage: {WeighedPercentage}",
                    attackGroup.Key,
                    count,
                    smoothedCount,
                    weighedPercentage);

                return new { Attack = attackGroup.Key, WeighedPercentage = weighedPercentage };
            })
            .OrderByDescending(a => a.WeighedPercentage)
            .ToList();

        var player1Prediction = attackPercentages[0].Attack;

        return ruleSetModule.GetAttackToBeat(player1Prediction);
    }

    private static decimal CalculateSmoothedCount(int attackGroupCount)
    {
        var smoothedCount = 1m;
        var alpha = 0.1m; // Smoothing factor

        // Apply exponential smoothing over the range of consecutive repeats
        for (var consecutiveRepeats = 1; consecutiveRepeats <= attackGroupCount; consecutiveRepeats++)
        {
            smoothedCount = (alpha * consecutiveRepeats) + ((1 - alpha) * smoothedCount);
        }

        return smoothedCount;
    }

    private static decimal CalculateWeightedPercentage(decimal smoothedCount, decimal totalCount)
    {
        if (totalCount == 0)
        {
            return 0m; // Avoid division by zero
        }

        // Calculate the weighted percentage
        return smoothedCount / totalCount;
    }
}