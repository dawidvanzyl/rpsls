using rpsls.Domain;
using System;

namespace rpsls.Infrastructure.Algorithms
{
    public class RandomAttackAlgorithm : AbstractAttackAlgorithm
    {
        public RandomAttackAlgorithm(Attack[] attacks)
            : base(attacks)
        {
        }

        public override Attack GetAttack()
        {
            var upscaledAttackLimit = (attacks.Length - 1) * 100;
            var randomizer = new Random();
            var nextAttackValue = randomizer.Next(1, upscaledAttackLimit);
            var downscaledNextAttackValue = Convert.ToByte(Math.Truncate(Convert.ToDecimal(nextAttackValue) / 100));
            var computerAttack = attacks[downscaledNextAttackValue];
            return computerAttack;
        }
    }
}