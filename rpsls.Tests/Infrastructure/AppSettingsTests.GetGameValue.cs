using FluentAssertions;
using rpsls.Domain;
using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using System;
using System.Collections.Generic;
using Xunit;

namespace rpsls.Tests.Infrastructure
{
    public partial class AppSettingsTests
    {
        [Fact]
        public void GetGameValue_Throws_ArgumentOutOfRangeException_When_Configuration_Not_In_Attacks()
        {
            //arrange
            var appSettings = Make_AppSettings("appsettings.invalidconfiguration.json");

            //action
            Action action = () => appSettings.GetGameValue();

            //assert
            action.Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage("Game configuration does not have an attack configuration. (Parameter 'Invalid')");
        }

        [Fact]
        public void GetGameValue_Throws_ArgumentOutOfRangeException_When_Attack_Configuration_Is_Invalid()
        {
            //arrange
            var appSettings = Make_AppSettings("appsettings.invalidgameattackconfig.json");

            //action
            Action action = () => appSettings.GetGameValue();

            //assert
            action.Should()
                .Throw<ArgumentOutOfRangeException>()
                .WithMessage($"Attack configuration cannot be transformed into attacks. (Parameter 'Invalid')");
        }

        [Fact]
        public void GetGameValue_Reads_Config()
        {
            //arrange
            var appSettings = Make_AppSettings("appsettings.gamevalue.json");

            var rockAttack = AttackValue.From("Rock", 1);
            var paperDefeatedByValue = DefeatedByValue.From(AttackValue.From("Paper", 2), "Paper covers Rock.");
            var spockDefeatedByValue = DefeatedByValue.From(AttackValue.From("Spock", 3), "Spock vaporizes Rock.");
            var attack = Attack.From(rockAttack, new List<DefeatedByValue> { paperDefeatedByValue, spockDefeatedByValue });
            var expectedGameValue = GameValue.From("Rock-Paper-Scissors", new List<Attack> { attack }, rounds: 10);


            //action
            var actualGameValue = appSettings.GetGameValue();

            //assert            
            actualGameValue.Should().BeEquivalentTo(expectedGameValue);
        }
    }
}
