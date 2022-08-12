using FluentAssertions;
using rpsls.Domain.Values;
using System;
using System.Collections.Generic;
using Xunit;

namespace rpsls.Tests.Infrastructure
{
    public partial class AppSettingsTests
    {
        [Fact]
        public void GetGameValue_Reads_Config()
        {
            //arrange
            var appSettings = Make_AppSettings("appsettings.gamevalue.json");

            var rockAttack = AttackValue.From("Rock", 1);
            var paperDefeatedByValue = DefeatedByValue.From(AttackValue.From("Paper", 2), "Paper covers Rock.");
            var spockDefeatedByValue = DefeatedByValue.From(AttackValue.From("Spock", 3), "Spock vaporizes Rock.");
            var challange = ChallangeValue.From(rockAttack, new List<DefeatedByValue> { paperDefeatedByValue, spockDefeatedByValue });
            var expectedGameValue = GameValue.From("Rock-Paper-Scissors", new List<ChallangeValue> { challange });

            //action
            var gameValues = appSettings.GetGames();

            //assert
            gameValues.Should().ContainEquivalentOf(expectedGameValue);
        }

        [Fact]
        public void GetGameValue_Throws_ArgumentOutOfRangeException_When_Attack_Configuration_Is_Invalid()
        {
            //arrange
            var appSettings = Make_AppSettings("appsettings.invalidgameattackconfig.json");

            //action
            Action action = () => appSettings.GetGames();

            //assert
            action.Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage($"Configuration cannot be transformed into challanges. (Parameter 'Invalid')");
        }
    }
}