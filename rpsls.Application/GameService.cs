using rpsls.Domain.Modules;
using rpsls.Entities;
using rpsls.Entities.Enums;

namespace rpsls.Application;

public interface IGameService
{
    Game Create(int bestOf);

    ResultTypes GetResult(AttackTypes p1, AttackTypes p2);

    int[] GetScores();

    bool IsOver();

    Task SaveAsync();
}

public class GameService(IGameModule gameModule, IRuleModule ruleModule)
    : IGameService
{
    private int _winningScore;

    public Game Create(int bestOf)
    {
        _winningScore = (int)Math.Round(bestOf * 0.66m);
        gameModule.Create(bestOf);

        return gameModule.Current;
    }

    public ResultTypes GetResult(AttackTypes p1, AttackTypes p2)
    {
        return p1 switch
        {
            _ when p1 == p2 => ResultTypes.Draw,
            _ when p1 == ruleModule.GetAttackToBeat(p2) => ResultTypes.Win,
            _ => ResultTypes.Loss
        };
    }

    public int[] GetScores()
    {
        return gameModule.Current.Scores;
    }

    public bool IsOver()
    {
        return gameModule.Current.Scores.Any(score => score == _winningScore);
    }

    public async Task SaveAsync()
    {
        await gameModule.SaveAsync();
    }
}