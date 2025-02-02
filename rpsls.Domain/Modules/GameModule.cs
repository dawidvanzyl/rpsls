using rpsls.Entities;
using rpsls.Infrastructure.Repositories;

namespace rpsls.Domain.Modules;

public interface IGameModule
{
    Game Current { get; }

    void Create(int bestOf);

    IList<Round> GetFullHistory();

    Task SaveAsync();
}

public class GameModule(IGameRepository gameRepository)
    : IGameModule
{
    private readonly Lazy<IList<Game>> _games =
        new Lazy<IList<Game>>(
            () => gameRepository
                    .GetAllAsync()
                    .GetAwaiter()
                    .GetResult());

    public Game Current { get; private set; }

    public void Create(int bestOf)
    {
        Current = new Game(bestOf, [0, 0], []);
    }

    public IList<Round> GetFullHistory()
    {
        return GetValue()
            .SelectMany(game => game.Rounds)
            .ToList();
    }

    public async Task SaveAsync()
    {
        await gameRepository.SaveAsync(
            Current.BestOf,
            Current.Scores[0],
            Current.Scores[1],
            Current.Rounds);

        GetValue().Add(Current);
    }

    private IList<Game> GetValue()
    {
        return _games == null
            ? throw new InvalidOperationException(nameof(_games))
            : _games.Value;
    }
}