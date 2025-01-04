using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Contexts;
using rpsls.Entities;
using rpsls.Entities.Enums;
using System.Collections.Immutable;

namespace rpsls.Domain.Algorithms.Modifiers.Abstracts
{
    public abstract class AbstractModifier(ILogger<AbstractModifier> logger)
        : IModifierAlgorithm
    {
        public IImmutableDictionary<AttackTypes, decimal> CalculatePercentages(IImmutableList<Match> matchHistory, AlgorithmContext context)
        {
            logger.LogDebug("Calculate weighed percentage");

            IImmutableDictionary<AttackTypes, decimal> attackPercentages = matchHistory
                .GroupBy((matchResult) => matchResult.P1Attack)
                .Select(attackGroup =>
                {
                    logger.LogTrace("Attack: {Attack}", attackGroup.Key);
                    logger.LogTrace("Count: {Count}", attackGroup.Count());

                    return new
                    {
                        Attack = attackGroup.Key,
                        WeighedPercentage = CalculateWeightedPercentage(attackGroup, context)
                    };
                })
                .ToImmutableDictionary(
                    a => a.Attack,
                    a => a.WeighedPercentage);

            return attackPercentages;
        }

        protected abstract decimal CalculateWeightedPercentage(IGrouping<AttackTypes, Match> attackGroup, AlgorithmContext context);
    }
}