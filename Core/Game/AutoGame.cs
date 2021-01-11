using rpsls.Attacks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Game
{
    public sealed class AutoGame : AbstractGame
    {
        private readonly Attack playerAttack;
        private readonly Random randomizer;

        private AutoGame(Attack playerAttack, GameConfig gameConfig, Attack[] attacks)
            : base(gameConfig, attacks)
        {
            this.playerAttack = playerAttack;
            this.randomizer = new Random();            
        }

        public override void Play()
        {
            for (short i = 0; i < gameConfig.NumberOfRounds; i++)
            {
                Console.WriteLine($"Round: {i + 1}");
                Console.WriteLine($"Player: {playerAttack.AttackType.Name}.");

                int upscaledAttackLimit = (gameConfig.AttackLimit + 1) * 100;
                var nextAttackValue = randomizer.Next(1, upscaledAttackLimit);
                var downscaledNextAttackValue = Convert.ToByte(Math.Truncate(Convert.ToDecimal(nextAttackValue) / 100));
                var attack = attacks[downscaledNextAttackValue];

                Console.WriteLine($"Computer: {attack.AttackType.Name}.");
                RecordAttack(i, playerAttack, attack);
            }
        }        

        public static AutoGame From(GameConfig gameConfig, Attack playerAttack, Attack[] attacks)
        {
            return new AutoGame(playerAttack, gameConfig, attacks);
        }
    }
}
