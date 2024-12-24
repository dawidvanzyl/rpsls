using rpsls.Entities.Enums;

namespace rpsls.Domain;

public class Match(int winningScore)
{
    private readonly int[] _scores = [0, 0];

    public ResultTypes GetResult(AttackTypes p1, AttackTypes p2)
    {
        if (p1 == p2)
        {
            return ResultTypes.Draw;
        }

        if ((p1 == AttackTypes.Rock && p2 == AttackTypes.Scissor)
            || (p1 == AttackTypes.Paper && p2 == AttackTypes.Rock)
            || (p1 == AttackTypes.Scissor && p2 == AttackTypes.Paper))
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