using FluentAssertions;
using Microsoft.Extensions.Configuration;
using rpsls.Domain;
using rpsls.Domain.Values;
using rpsls.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace rpsls.Tests.Infrastructure
{
    public partial class AppSettingsTests
    {
        [Fact]
        public void GetAttacks_Throws_ArgumentOutOfRangeException_When_AttackType_Invalid()
        {
            //arrange
            var appSettings = Make_AppSettings("appsettings.brokenattacks.json");

            //action
            Action action = () => appSettings.GetAttacks();

            //assert
            action.Should()
                .Throw<InvalidDataException>()
                .WithMessage("Attacks configuration contains invalid attack values");
        }

        [Fact]
        public void GetAttacks_Throws_ArgumentOutOfRangeException_When_DefeatedBy_AttackType_Invalid()
        {
            //arrange
            var appSettings = Make_AppSettings("appsettings.brokendefeatedby.json");

            //action
            Action action = () => appSettings.GetAttacks();

            //assert
            action.Should()
                .Throw<InvalidDataException>()
                .WithMessage($"DefeatedBy configuration for [Rock] contains invalid attack values");
        }

        [Fact]
        public void GetAttacks_Reads_Config()
        {
            //arrange
            var appSettings = Make_AppSettings("appsettings.attacks.json");

            var rockAttack = AttackValue.From("Rock", 1);
            var paperDefeatedByValue = DefeatedByValue.From(AttackValue.From("Paper", 2), "Paper covers Rock.");
            var spockDefeatedByValue = DefeatedByValue.From(AttackValue.From("Spock", 3), "Spock vaporizes Rock.");
            var attack = Attack.From(rockAttack, new List<DefeatedByValue> { paperDefeatedByValue, spockDefeatedByValue });

            //action
            var attacks = appSettings.GetAttacks();

            //assert
            attacks.Count.Should().Be(1);
            attacks.Should().Contain(attack);
        }
    }    
}
