using rpsls.Attacks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Game
{
    public abstract class AbstractGame
    {
        protected readonly GameConfig gameConfig;
        protected readonly Attack[] attacks;

        private IDictionary<short, AttackResult> gameResults;

        protected AbstractGame(GameConfig gameConfig, Attack[] attacks)
        {
            gameResults = new Dictionary<short, AttackResult>();
            this.gameConfig = gameConfig;
            this.attacks = attacks;

            Console.WriteLine(gameConfig.Name);
        }

        public abstract void Play();

        protected void RecordAttack(short round, Attack playerAttack, Attack computerAttack)
        {
            var attackResult = (AttackResult)playerAttack.CompareTo(computerAttack);
            gameResults.Add(round, attackResult);

            string message = null;
            switch (attackResult)
            {
                case AttackResult.Win:
                    message = $"wins: {computerAttack.GetDefeatMessage(playerAttack.AttackType)}";
                    break;
                case AttackResult.Lose:
                    message = $"loses: {playerAttack.GetDefeatMessage(computerAttack.AttackType)}";
                    break;
                case AttackResult.Tie:
                    message = $"tied.";
                    break;
            }

            Console.WriteLine($"Player {message}");
            Console.WriteLine("");
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
    }
}
