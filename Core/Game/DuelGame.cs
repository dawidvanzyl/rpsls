using rpsls.Attacks;

namespace rpsls.Game
{
    public sealed class DuelGame : AbstractGame
    {
        public DuelGame(GameConfig gameConfig, Attack[] attacks) 
            : base(gameConfig, attacks)
        {

        }

        public override void Play()
        {
            throw new System.NotImplementedException();
        }
    }
}
