using rpsls.Domain.Modules;
using rpsls.Entities.Enums;

namespace rpsls.Domain;

public class Match(int winningScore, IRuleModule ruleModule)
{
    private readonly int[] _scores = [0, 0];

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
        return _scores.Any(score => score == winningScore);
    }
}