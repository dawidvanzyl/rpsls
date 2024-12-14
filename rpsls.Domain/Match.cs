using rpsls.Domain.Enums;

namespace rpsls.Domain
{
    public class Match
    {
        private readonly int[] _scores;
        private readonly int _winningScore;

        public Match(int winningScore)
        {
            _winningScore = winningScore;
            _scores = [0, 0];
        }

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
            return _scores.Any(score => score == _winningScore);
        }
    }
}