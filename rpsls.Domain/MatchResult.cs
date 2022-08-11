using rpsls.Domain.Enums;
using System;

namespace rpsls.Domain
{
    public class MatchResult
    {
        public Guid Id { get; set; }

        public PlayerResult PlayerOne { get; set; }

        public PlayerResult PlayerTwo { get; set; }

        public static MatchResult From(Attack playerOne, Attack playerTwo, AttackResult attackResult)
        {
            var playerTwoResult = ReverseAttackResult(attackResult);

            return new MatchResult
            {
                Id = Guid.NewGuid(),
                PlayerOne = new PlayerResult
                {
                    Id = $"{playerOne.AttackValue.Value}-{attackResult}",
                    PlayerAttack = playerOne.AttackValue,
                    AttackResult = attackResult
                },
                PlayerTwo = new PlayerResult
                {
                    Id = $"{playerTwo.AttackValue.Value}-{playerTwoResult}",
                    PlayerAttack = playerTwo.AttackValue,
                    AttackResult = playerTwoResult
                }
            };
        }

        private static AttackResult ReverseAttackResult(AttackResult attackResult)
        {
            return attackResult switch
            {
                AttackResult.Won => AttackResult.Lost,
                AttackResult.Lost => AttackResult.Won,
                _ => AttackResult.Tied
            };
        }
    }
}