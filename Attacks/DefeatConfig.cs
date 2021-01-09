namespace rpsls.Attacks
{
    public sealed class DefeatConfig
    {
        private DefeatConfig(AttackType attackType, string message)
        {
            AttackType = attackType;
            Message = message;
        }

        public AttackType AttackType { get; }
        public string Message { get; }

        public static DefeatConfig From(AttackType attackType, string message)
        {
            return new DefeatConfig(attackType, message);
        }
    }
}
