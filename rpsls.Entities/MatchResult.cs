using rpsls.Entities.Enums;

namespace rpsls.Entities
{
    public class MatchResult
    {
        public bool IsNew { get; init; }
        public AttackTypes Player1 { get; init; }
        public AttackTypes Player2 { get; init; }
        public ResultTypes Result { get; init; }

        public static MatchResult Create(AttackTypes p1, AttackTypes p2, ResultTypes result)
        {
            return new MatchResult
            {
                IsNew = true,
                Player1 = p1,
                Player2 = p2,
                Result = result
            };
        }
    }
}