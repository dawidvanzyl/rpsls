using rpsls.Entities.Enums;

namespace rpsls.Entities
{
    public record Game(
        int BestOf,
        int[] Scores,
        IList<Round> Rounds
        )
    {
        public void AddRound(AttackTypes p1, AttackTypes p2, ResultTypes result)
        {
            var lastResult = Rounds.LastOrDefault();
            var consecutiveRepeats = lastResult == null || lastResult.P1Attack != p1
                ? 1
                : lastResult.ConsecutiveRepeats + 1;

            Rounds.Add(new Round(
                Rounds.Count + 1,
                p1,
                p2,
                result,
                consecutiveRepeats));

            if (result == ResultTypes.Win)
            {
                Scores[0]++;
            }

            if (result == ResultTypes.Loss)
            {
                Scores[1]++;
            }
        }
    }
}