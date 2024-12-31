using rpsls.Entities;
using System.Collections.Immutable;

namespace rpsls.Domain.Algorithms.Contexts;

public class AlgorithmContext
{
    public Match LastMatch { get; init; }

    public IImmutableList<Match> MatchHistory { get; init; }

    public int TotalCount { get; init; }
}