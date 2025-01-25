using rpsls.Entities;
using rpsls.Entities.Enums;
using System.Collections.Immutable;

namespace rpsls.Domain.Algorithms.Weighted;

public interface IWeightingAlgorithm
{
    IImmutableDictionary<AttackTypes, decimal> CalculateWeights(IImmutableList<Match> matchHistory);
}