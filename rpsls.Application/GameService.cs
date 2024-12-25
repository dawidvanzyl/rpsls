using rpsls.Domain;
using rpsls.Domain.Algorithms;
using rpsls.Domain.Modules;

namespace rpsls.Application;

public interface IGameService
{
    Match CreateMatch(int bestOf);

    IAlgorithm GetAlgorithm();

    Task SaveMatchResultsAsync();
}

public class GameService(
    IMatchResultModule matchResultModule,
    IRuleModule ruleModule,
    IAlgorithm algorithm)
    : IGameService
{
    public Match CreateMatch(int bestOf)
    {
        var winningScore = (int)Math.Round(bestOf * 0.66m);
        return new Match(winningScore, ruleModule);
    }

    public IAlgorithm GetAlgorithm()
    {
        return algorithm;
    }

    public async Task SaveMatchResultsAsync()
    {
        await matchResultModule.SaveAsync();
    }
}