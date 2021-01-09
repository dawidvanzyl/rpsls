using rpsls.Attacks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rpsls
{
    public class Game
    {
        private readonly GameConfig gameConfig;
        private readonly Attack playerAttack;
        private readonly Attack[] attacks;
        private readonly Random randomizer;
        private readonly Dictionary<short, AttackResult> gameResults;

        private Game(GameConfig gameConfig, Attack playerAttack, Attack[] attacks)
        {
            this.gameConfig = gameConfig;
            this.playerAttack = playerAttack;
            this.attacks = attacks;
            this.randomizer = new Random();
            gameResults = new Dictionary<short, AttackResult>();
        }

        public void Play()
        {
            Console.WriteLine(gameConfig.Name);
            for (short i = 0; i < gameConfig.NumberOfRounds; i++)
            {
                Console.WriteLine($"Round: {i + 1}");
                Console.WriteLine($"Player: {playerAttack.AttackType.Name}.");

                int upscaledAttackLimit = (gameConfig.AttackLimit + 1) * 100;
                var nextAttackValue = randomizer.Next(1, upscaledAttackLimit);
                var downscaledNextAttackValue = Convert.ToByte(Math.Truncate(Convert.ToDecimal(nextAttackValue) / 100));
                var attack = attacks[downscaledNextAttackValue];

                Console.WriteLine($"Computer: {attack.AttackType.Name}.");

                var attackResult = playerAttack.GetAttackResult(attack.AttackType);
                gameResults.Add(i, attackResult);

                string message = null;
                switch (attackResult)
                {
                    case AttackResult.Win:
                        message = $"wins: {attack.GetDefeatMessage(playerAttack.AttackType)} ({nextAttackValue}).";
                        break;
                    case AttackResult.Lose:
                        message = $"loses: {playerAttack.GetDefeatMessage(attack.AttackType)} ({nextAttackValue}).";
                        break;
                    case AttackResult.Tie:
                        message = $"tied ({nextAttackValue}).";
                        break;
                }

                Console.WriteLine($"Player {message}");
                Console.WriteLine("");
            }
        }

        public void PrintStats()
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine($"Games played: {gameResults.Count}"); Console.WriteLine($"Games played: {gameResults.Count}");

            var gamesPlayed = Convert.ToDecimal(gameResults.Count);
            var gamesTied = gameResults.Where(gameResult => gameResult.Value == AttackResult.Tie).Count();
            var gamesWon = gameResults.Where(gameResult => gameResult.Value == AttackResult.Win).Count();
            var gamesLost = gameResults.Where(gameResult => gameResult.Value == AttackResult.Lose).Count();
            Console.WriteLine($"Games tied: {gamesTied} ({Math.Round((Convert.ToDecimal(gamesTied) / gameResults.Count) * 100, 2)}%)");
            Console.WriteLine($"Games won: {gamesWon} ({Math.Round((Convert.ToDecimal(gamesWon) / gameResults.Count) * 100, 2)}%)");
            Console.WriteLine($"Games lost: {gamesLost} ({Math.Round((Convert.ToDecimal(gamesLost) / gameResults.Count) * 100, 2)}%)");
        }

        public static Game From(GameConfig gameConfig, Attack playerAttack, Attack[] attacks)
        {
            return new Game(gameConfig, playerAttack, attacks);
        }
    }
}
