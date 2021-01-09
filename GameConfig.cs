namespace rpsls
{
    public sealed class GameConfig
    {
        private GameConfig(string name, byte attackLimit, short numberOfRounds)
        {
            Name = name;
            AttackLimit = attackLimit;
            NumberOfRounds = numberOfRounds;
        }

        public string Name { get; }
        public short AttackLimit { get; }
        public short NumberOfRounds { get; }

        public static GameConfig From(string name, byte attackLimit, short numberOfRounds)
        {
            return new GameConfig(name, attackLimit, numberOfRounds);
        }
    }
}
