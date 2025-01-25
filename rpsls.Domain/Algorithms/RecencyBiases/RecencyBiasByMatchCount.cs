using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.RecencyBiases.Abstracts;
using rpsls.Entities;
using rpsls.Entities.Enums;
using System.Collections.Immutable;

namespace rpsls.Domain.Algorithms.RecencyBiases;

public class RecencyBiasByMatchCount(ILogger<RecencyBiasByMatchCount> logger)
    : AbstactRecencyBiasAlgorithm(logger)
{
    protected override decimal GetBoostedPercentage(
        AttackTypes attack,
        decimal percentage,
        decimal recencyBiasFactor,
        IImmutableList<Match> matchHistory)
    {
        var recentMatchBias = recencyBiasFactor / matchHistory.Count;
        var recencyBiasBoost = 1 + recentMatchBias;
        var boostedPercentage = percentage * recencyBiasBoost;

        logger.LogTrace("Recency Bais Boost: {RecencyBiasBoost}", recencyBiasBoost);
        logger.LogTrace("Boosted Percentage: {BoostedPercentage}", boostedPercentage);

        return boostedPercentage;
    }
}