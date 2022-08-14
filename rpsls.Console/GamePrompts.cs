using rpsls.Application;
using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using rpsls.Infrastructure.Repositories;
using Sharprompt;
using System;
using System.Collections.Generic;

namespace rpsls.Console
{
    public class GamePrompts
    {
        private readonly GameServiceFactory gameServiceFactory;
        private readonly IList<GameValue> gameValues;

        public GamePrompts(ISettingsRepository settingsRepository, GameServiceFactory gameServiceFactory)
        {
            gameValues = settingsRepository.GetGames();
            this.gameServiceFactory = gameServiceFactory;
        }

        public void Play()
        {
            var twoPlayerGame = !Prompt.Confirm("Do you want to play against the computer?");
            var player1Name = Prompt.Input<string>("Player 1 what's your name?");
            var player2Name = twoPlayerGame
                ? Prompt.Input<string>("Player 2 what's your name?")
                : "Computer";

            var gameValue = Prompt.Select($"{player1Name} please select the game you want to play", gameValues);
            var rounds = Prompt.Input<byte>("How many rounds would you like to play (1-255)?");
            if (rounds == 0)
            {
                System.Console.WriteLine($"Sorry {player1Name} we cannot do zero rounds, I'll upgrade you to 1 round.");
                rounds = 1;
            }

            System.Console.WriteLine("");
            System.Console.WriteLine($"{gameValue}");
            System.Console.WriteLine("");

            var gameService = gameServiceFactory.Create(gameValue);
            for (byte i = 0; i < rounds; i++)
            {
                System.Console.WriteLine($"Round {i + 1}");

                if (twoPlayerGame)
                {
                    Prompt.ColorSchema.Answer = ConsoleColor.Black;
                }

                var player1 = Prompt.Select($"{player1Name} please select you attack", gameValue.Challenges);

                ChallengeValue player2;
                if (twoPlayerGame)
                {
                    player2 = Prompt.Select($"{player2Name} please select you attack", gameValue.Challenges);
                }
                else
                {
                    System.Console.WriteLine("Computer is deciding on an attack...");
                    System.Console.WriteLine("");
                    player2 = gameService.GetChallenge();
                }

                System.Console.WriteLine("");
                System.Console.WriteLine($"{player1Name} attacked with {player1}");
                System.Console.WriteLine($"{player2Name} attacked with {player2}");
                System.Console.WriteLine("");

                var challengeResult = (ChallengeResult)player1.CompareTo(player2);
                gameService.SaveMatchResult(player1, player2, challengeResult);
                switch (challengeResult)
                {
                    case ChallengeResult.Won:
                        System.Console.WriteLine(player2.GetDefeatMessage(player1.Attack));
                        System.Console.WriteLine($"{player1Name} {ChallengeResult.Won.ToString().ToUpper()}");
                        break;

                    case ChallengeResult.Tied:
                        System.Console.WriteLine($"Match {ChallengeResult.Tied.ToString().ToUpper()}");
                        break;

                    case ChallengeResult.Lost:
                        System.Console.WriteLine(player1.GetDefeatMessage(player2.Attack));
                        System.Console.WriteLine($"{player2Name} {ChallengeResult.Won.ToString().ToUpper()}");
                        break;
                }

                System.Console.WriteLine("");
            }
        }
    }
}