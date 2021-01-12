using rpsls.Application;
using rpsls.Application.Entities;
using rpsls.Application.Repositories;
using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using Sharprompt;
using System;
using System.Collections.Generic;

namespace rpsls.Console
{
    public class GamePrompts
    {
        private readonly IList<GameValue> gameValues;
        private readonly GameServiceFactory gameServiceFactory;

        public GamePrompts(ISettingsRepository settingsRepository, GameServiceFactory gameServiceFactory)
        {
            gameValues = settingsRepository.GetGameValues();
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
            MatchResult previousMatchResult = null;
            for (byte i = 0; i < rounds; i++)
            {
                System.Console.WriteLine($"Round {i + 1}");

                if (twoPlayerGame)
                {
                    Prompt.ColorSchema.Answer = ConsoleColor.Black;
                }

                var player1Attack = Prompt.Select($"{player1Name} please select you attack", gameValue.Attacks);
                var player2Attack = twoPlayerGame
                    ? Prompt.Select($"{player2Name} please select you attack", gameValue.Attacks)
                    : gameService.PredictAttack(previousMatchResult);

                var matchResult = gameService.RecordMatchResult(player1Attack, player2Attack, previousMatchResult);
                previousMatchResult = matchResult;

                System.Console.WriteLine("");
                System.Console.WriteLine($"{player1Name} attacked with {player1Attack}");
                System.Console.WriteLine($"{player2Name} attacked with {player2Attack}");
                System.Console.WriteLine("");

                switch (matchResult.AttackResult)
                {
                    case AttackResult.Won:
                        System.Console.WriteLine(player2Attack.GetDefeatMessage(player1Attack.AttackValue));
                        System.Console.WriteLine($"{player1Name} {AttackResult.Won.ToString().ToUpper()}");
                        break;
                    case AttackResult.Tied:
                        System.Console.WriteLine($"Match {AttackResult.Tied.ToString().ToUpper()}");
                        break;
                    case AttackResult.Lost:
                        System.Console.WriteLine(player1Attack.GetDefeatMessage(player2Attack.AttackValue));
                        System.Console.WriteLine($"{player2Name} {AttackResult.Won.ToString().ToUpper()}");
                        break;
                }

                System.Console.WriteLine("");
            }
        }
    }
}
