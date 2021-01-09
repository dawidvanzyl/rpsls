using Microsoft.Extensions.Configuration;
using rpsls.Attacks;

namespace rpsls
{
    public class GameFactory
    {
        private readonly IConfiguration configuration;

        public GameFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Game GetGame(Attack playerAttack, Attack[] attacks)
        {
            var gameType = configuration.GetSection("ActiveGame").GetValue<string>("Type");
            var rounds = configuration.GetSection("ActiveGame").GetValue<short>("Rounds");
            var attackLimit = configuration.GetSection("Games").GetValue<byte>(gameType);

            var gameConfig = GameConfig.From(gameType, attackLimit, rounds);
            return Game.From(gameConfig, playerAttack, attacks);
        }
    }
}
