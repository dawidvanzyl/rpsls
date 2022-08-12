using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using System;

namespace rpsls.Domain
{
    public class MatchResult
    {
        public Guid Id { get; set; }

        public PlayerResult PlayerOne { get; set; }

        public PlayerResult PlayerTwo { get; set; }

        public static MatchResult From(ChallangeValue playerOne, ChallangeValue playerTwo, ChallangeResult challangeResult)
        {
            var playerTwoResult = ReverseChallangeResult(challangeResult);

            return new MatchResult
            {
                Id = Guid.NewGuid(),
                PlayerOne = new PlayerResult
                {
                    Id = $"{playerOne.Attack.Value}-{challangeResult}",
                    PlayerAttack = playerOne.Attack,
                    ChallangeResult = challangeResult
                },
                PlayerTwo = new PlayerResult
                {
                    Id = $"{playerTwo.Attack.Value}-{playerTwoResult}",
                    PlayerAttack = playerTwo.Attack,
                    ChallangeResult = playerTwoResult
                }
            };
        }

        private static ChallangeResult ReverseChallangeResult(ChallangeResult challangeResult)
        {
            return challangeResult switch
            {
                ChallangeResult.Won => ChallangeResult.Lost,
                ChallangeResult.Lost => ChallangeResult.Won,
                _ => ChallangeResult.Tied
            };
        }
    }
}