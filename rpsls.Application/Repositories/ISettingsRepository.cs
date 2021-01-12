using rpsls.Domain;
using rpsls.Domain.Values;
using System.Collections.Generic;

namespace rpsls.Application.Repositories
{
    public interface ISettingsRepository
    {
        IList<AttackValue> GetAttackValues();

        IList<Attack> GetAttacks();

        IList<GameValue> GetGameValues();
    }
}
