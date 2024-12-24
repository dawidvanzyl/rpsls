using rpsls.Entities;
using rpsls.Entities.Enums;
using rpsls.Infrastructure.Repositories;
using System.Collections.Immutable;

namespace rpsls.Domain.Modules;

public interface IMatchResultModule
{
    void Add(AttackTypes p1, AttackTypes p2, ResultTypes result);

    IImmutableList<MatchResult> GetAll();

    Task SaveAsync();
}

public class MatchResultModule(IMatchResultRepository matchResultRepository) : IMatchResultModule
{
    private readonly Lazy<IList<MatchResult>> _matchResults =
        new Lazy<IList<MatchResult>>(
            () => matchResultRepository
                    .GetAllAsync()
                    .GetAwaiter()
                    .GetResult());

    public void Add(AttackTypes p1, AttackTypes p2, ResultTypes result)
    {
        var lastMatchResult = GetValue().LastOrDefault();
        var attackCount = lastMatchResult == null || lastMatchResult.P1Attack != p1
            ? 1
            : lastMatchResult.ConsecutiveRepeats + 1;

        GetValue().Add(new MatchResult(
            attackCount,
            true,
            p1,
            p2,
            result));
    }

    public IImmutableList<MatchResult> GetAll()
    {
        return GetValue().ToImmutableList();
    }

    public async Task SaveAsync()
    {
        await matchResultRepository.CreateAsync(GetValue().Where(matchResult => matchResult.IsNew));
    }

    private IList<MatchResult> GetValue()
    {
        return _matchResults == null
            ? throw new InvalidOperationException(nameof(_matchResults))
            : _matchResults.Value;
    }
}