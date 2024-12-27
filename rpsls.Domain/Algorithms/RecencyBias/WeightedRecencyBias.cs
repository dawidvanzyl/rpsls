using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Models;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.RecencyBias;

public class WeightedRecencyBias(ILogger<WeightedRecencyBias> logger)
    : IRecencyBiasAlgorithm<WeightedRecencyBiasInput>
{
    public decimal ApplyRecencyBias(WeightedRecencyBiasInput input)
    {
        var recencyBais = input.LastResult switch
        {
            ResultTypes.Win => 0.3m,
            ResultTypes.Draw => 0.15m,
            _ => -0.075m
        };

        var recencyBiasBoost = 1 + recencyBais;
        var boostedPercentage = input.WeighedPercentage * recencyBiasBoost;

        logger.LogDebug("Recency Bais Boost: {RecencyBiasBoost}", recencyBiasBoost);
        logger.LogDebug("Boosted Percentage: {BoostedPercentage}", boostedPercentage);

        return boostedPercentage;
    }
}