using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.RecencyBiases.Abstracts;
using rpsls.Domain.Modules;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.RecencyBiases;

public class ExponentialDecayRecencyBias(ILogger<ExponentialDecayRecencyBias> logger, IGameModule gameModule)
    : AbstactRecencyBiasAlgorithm(logger, gameModule)
{
    protected override decimal GetBoostedPercentage(AttackTypes attack, decimal percentage, decimal recencyBiasFactor)
    {
        var attackHistory = GameModule.Current.Rounds
            .Where(match => match.P1Attack == attack)
            .ToList();

        var roundsSinceLastMatch = attackHistory[^1].Number - attackHistory[^2].Number; // Number of matches since this attack was used
        var decayRate = 0.1m; // Lambda for exponential decay
        var decayedBias = recencyBiasFactor * (decimal)Math.Exp((double)(-decayRate * roundsSinceLastMatch));
        var boostedPercentage = percentage + decayedBias;

        logger.LogTrace("Decay Rate: {DecayRate}", decayRate);
        logger.LogTrace("Decayed Bias: {DecayedBias}", decayedBias);
        logger.LogTrace("Boosted Percentage: {BoostedPercentage}", boostedPercentage);

        return boostedPercentage;
    }
}