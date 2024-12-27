using rpsls.Domain.Modules;
using rpsls.Entities.Enums;

namespace rpsls.Application;

public interface IGameService
{
    void CreateMatch(int bestOf);

    ResultTypes GetResult(AttackTypes p1, AttackTypes p2);

    int[] GetScores();

    bool IsOver();

    Task SaveMatchResultsAsync();
}

public class GameService(IMatchModule matchModule, IRuleModule ruleModule)
    : IGameService
{
    private readonly int[] _scores = [0, 0];
    private int _winningScore;

    public void CreateMatch(int bestOf)
    {
        _winningScore = (int)Math.Round(bestOf * 0.66m);
    }

    public ResultTypes GetResult(AttackTypes p1, AttackTypes p2)
    {
        if (p1 == p2)
        {
            return ResultTypes.Draw;
        }

        var winningAttack = ruleModule.GetAttackToBeat(p2);

        if (winningAttack == p1)
        {
            _scores[0]++;
            return ResultTypes.Win;
        }

        _scores[1]++;
        return ResultTypes.Loss;
    }

    public int[] GetScores()
    {
        return _scores;
    }

    public bool IsOver()
    {
        return _scores.Any(score => score == _winningScore);
    }

    public async Task SaveMatchResultsAsync()
    {
        await matchModule.SaveAsync();
    }
}