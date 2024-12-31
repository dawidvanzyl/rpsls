using rpsls.Entities;
using rpsls.Entities.Enums;
using rpsls.Infrastructure.Repositories;
using System.Collections.Immutable;

namespace rpsls.Domain.Modules;

public interface IMatchModule
{
    void Add(AttackTypes p1, AttackTypes p2, ResultTypes result);

    IImmutableList<Match> GetAll();

    Task SaveAsync();
}

public class MatchModule(IMatchRepository matchRepository)
    : IMatchModule
{
    private readonly Lazy<IList<Match>> _matches =
        new Lazy<IList<Match>>(
            () => matchRepository
                    .GetAllAsync()
                    .GetAwaiter()
                    .GetResult());

    public void Add(AttackTypes p1, AttackTypes p2, ResultTypes result)
    {
        var lastMatchResult = GetValue().LastOrDefault();
        var attackCount = lastMatchResult == null || lastMatchResult.P1Attack != p1
            ? 1
            : lastMatchResult.ConsecutiveRepeats + 1;

        GetValue().Add(new Match(
            attackCount,
            true,
            p1,
            p2,
            result,
            GetValue().Count + 1));
    }

    public IImmutableList<Match> GetAll()
    {
        return GetValue().ToImmutableList();
    }

    public async Task SaveAsync()
    {
        await matchRepository.CreateAsync(GetValue().Where(matchResult => matchResult.IsNew));
    }

    private IList<Match> GetValue()
    {
        return _matches == null
            ? throw new InvalidOperationException(nameof(_matches))
            : _matches.Value;
    }
}