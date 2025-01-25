using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.RecencyBiases.Abstracts;
using rpsls.Entities;
using rpsls.Entities.Enums;
using System.Collections.Immutable;

namespace rpsls.Domain.Algorithms.RecencyBiases;

public class NormalizedRecencyBias(ILogger<NormalizedRecencyBias> logger)
    : AbstactRecencyBiasAlgorithm(logger)
{
    protected override decimal GetBoostedPercentage(
        AttackTypes attack,
        decimal percentage,
        decimal recencyBiasFactor,
        IImmutableList<Match> matchHistory)
    {
        var maxPercentage = 100m; // Assume the percentage is normalized to 0-100
        var normalizedBias = recencyBiasFactor * (percentage / maxPercentage);

        logger.LogTrace("Recency Bias Factor: {RecencyBiasFactor}", recencyBiasFactor);
        logger.LogTrace("Max Percentage: {MaxPercentage}", maxPercentage);
        logger.LogTrace("Normalized Bias: {NormalizedBias}", normalizedBias);

        return normalizedBias;
    }
}