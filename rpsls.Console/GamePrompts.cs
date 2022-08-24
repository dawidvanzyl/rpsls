using rpsls.Application;
using rpsls.Application.Models;
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
            var gameValue = Prompt.Select("Please select the game you want to play", gameValues);
            var rounds = Prompt.Input<byte>("How many rounds would you like to play (1-255)?");
            if (rounds == 0)
            {
                System.Console.WriteLine($"Sorry we cannot do zero rounds, I'll upgrade you to 1 round.");
                rounds = 1;
            }

            var noPlayerGame = Prompt.Confirm("Do you want to the computer to itself?");
            var twoPlayerGame = false;
            if (!noPlayerGame)
            {
                twoPlayerGame = !Prompt.Confirm("Do you want to play against the computer?");
            }

            var playerOne = new PlayerInput();
            var playerTwo = new PlayerInput();

            playerOne.Name = !noPlayerGame
                ? Prompt.Input<string>("Player 1 what's your name?")
                : "Random-1";

            playerTwo.Name = twoPlayerGame
                ? Prompt.Input<string>("Player 2 what's your name?")
                : "ML-2";

            System.Console.WriteLine("");
            System.Console.WriteLine($"{gameValue}");
            System.Console.WriteLine("");

            var gameService = gameServiceFactory.Create(gameValue);
            gameService.StartMatch(rounds, playerOne, playerTwo);

            for (byte i = 0; i < rounds; i++)
            {
                System.Console.WriteLine($"Round {i + 1}");

                if (twoPlayerGame)
                {
                    Prompt.ColorSchema.Answer = ConsoleColor.Black;
                }

                if (!noPlayerGame)
                {
                    playerOne.Challenge = Prompt.Select($"{playerOne.Name} please select you attack", gameValue.Challenges);
                }
                else
                {
                    System.Console.WriteLine($"{playerOne.Name} is deciding on an attack...");
                    playerOne.Challenge = gameService.GetRandomChallenge();
                }

                if (twoPlayerGame)
                {
                    playerTwo.Challenge = Prompt.Select($"{playerTwo.Name} please select you attack", gameValue.Challenges);
                }
                else
                {
                    System.Console.WriteLine($"{playerTwo.Name} is deciding on an attack...");
                    playerTwo.Challenge = gameService.GetMLChallenge();
                }

                System.Console.WriteLine("");
                System.Console.WriteLine($"{playerOne.Name} attacked with {playerOne.Challenge}");
                System.Console.WriteLine($"{playerTwo.Name} attacked with {playerTwo.Challenge}");
                System.Console.WriteLine("");

                var roundResult = gameService.GetRoundResult(playerOne, playerTwo);
                if (!string.IsNullOrEmpty(roundResult.DefeatMessage))
                {
                    System.Console.WriteLine(roundResult.DefeatMessage);
                }

                System.Console.WriteLine(roundResult.ChallengeMessage);
                System.Console.WriteLine("");

                gameService.SaveRoundResult(roundResult);
            }

            var matchResult = gameService.EndMatch();
            System.Console.WriteLine(matchResult.GetMatchMessage());
        }
    }
}