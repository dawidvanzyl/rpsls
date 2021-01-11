using FluentAssertions;
using rpsls.Domain.Values;
using Xunit;

namespace rpsls.Tests.Infrastructure
{
    public partial class AppSettingsTests
    {
        [Fact]
        public void GetAttackValues_Reads_Config()
        {
            //arrange
            var appSettings = Make_AppSettings("appsettings.attackvalues.json");

            //action
            var attackTypes = appSettings.GetAttackValues();

            //assert
            attackTypes.Count.Should().Be(5);
            attackTypes.IndexOf(AttackValue.From("Rock", 1)).Should().Be(0);
            attackTypes.IndexOf(AttackValue.From("Paper", 2)).Should().Be(1);
            attackTypes.IndexOf(AttackValue.From("Scissors", 3)).Should().Be(2);
            attackTypes.IndexOf(AttackValue.From("Lizard", 4)).Should().Be(3);
            attackTypes.IndexOf(AttackValue.From("Spock", 5)).Should().Be(4);
        }
    }
}
