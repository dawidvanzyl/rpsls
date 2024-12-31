using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Contexts;
using rpsls.Domain.Algorithms.Modifiers.Abstracts;
using rpsls.Entities;
using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.Modifiers
{
    public class LogarithmicModifier(ILogger<LogarithmicModifier> logger)
        : AbstractModifier(logger)
    {
        protected override decimal CalculateWeightedPercentage(IGrouping<AttackTypes, Match> attackGroup, AlgorithmContext context)
        {
            var logarithmicCount = Math.Log(attackGroup.Sum(match => match.ConsecutiveRepeats) + 1);

            // Calculate the weighted percentage
            var weightedPercentage = context.TotalCount / logarithmicCount;

            logger.LogDebug("Logarithmic Count: {LogarithmicCount}", logarithmicCount);
            logger.LogDebug("Weighted Percentage: {WeightedPercentage}", weightedPercentage);

            return (decimal)weightedPercentage;
        }
    }
}