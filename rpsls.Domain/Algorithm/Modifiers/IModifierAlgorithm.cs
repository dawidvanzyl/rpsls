using rpsls.Domain.Algorithms.Contexts;
using rpsls.Entities;
using rpsls.Entities.Enums;
using System.Collections.Immutable;

namespace rpsls.Domain.Algorithms.Modifiers;

public interface IModifierAlgorithm
{
    IImmutableDictionary<AttackTypes, decimal> CalculatePercentages(IImmutableList<Match> matchHistory, AlgorithmContext context);
}