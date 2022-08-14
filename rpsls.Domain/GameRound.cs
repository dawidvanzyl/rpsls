﻿using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using System;

namespace rpsls.Domain
{
    public class GameRound
    {
        public Guid Id { get; set; }

        public Challenge PlayerOne { get; set; }

        public Challenge PlayerTwo { get; set; }

        public static GameRound From(ChallengeValue playerOne, ChallengeValue playerTwo, ChallengeResult challengeResult)
        {
            var playerTwoResult = ReverseChallengeResult(challengeResult);

            return new GameRound
            {
                Id = Guid.NewGuid(),
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
                }
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