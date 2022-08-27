using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using System;

namespace rpsls.Domain
{
    public class GameRound
    {
        public string GameName { get; set; }

        public Guid Id { get; set; }

        public DateTime Played { get; set; }

        public Challenge PlayerOne { get; set; }

        public Challenge PlayerTwo { get; set; }

        public static GameRound From(GameValue gameValue, ChallengeValue playerOne, ChallengeValue playerTwo, ChallengeResult challengeResult)
        {
            var playerTwoResult = ReverseChallengeResult(challengeResult);

            return new GameRound
            {
                Id = Guid.NewGuid(),
                GameName = gameValue.Name,
                PlayerOne = new Challenge
                {
                    Id = $"{playerOne.Attack.Value}-{challengeResult}",
                    AttackValue = playerOne.Attack.Value,
                    ChallengeResult = challengeResult
                },
                PlayerTwo = new Challenge
                {
                    Id = $"{playerTwo.Attack.Value}-{playerTwoResult}",
                    AttackValue = playerTwo.Attack.Value,
                    ChallengeResult = playerTwoResult
                },
                Played = DateTime.Now
            };
        }

        private static ChallengeResult ReverseChallengeResult(ChallengeResult challengeResult)
        {
            return challengeResult switch
            {
                ChallengeResult.Won => ChallengeResult.Lost,
                ChallengeResult.Lost => ChallengeResult.Won,
                _ => ChallengeResult.Tied
            };
        }
    }
}