using FluentAssertions;
using rpsls.Domain.Values;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace rpsls.Tests.Infrastructure
{
    public partial class AppSettingsTests
    {
        [Fact]
        public void GetAttacks_Reads_Config()
        {
            //arrange
            var appSettings = Make_AppSettings("appsettings.attacks.json");

            var rockAttack = AttackValue.From("Rock", 1);
            var paperDefeatedByValue = DefeatedByValue.From(AttackValue.From("Paper", 2), "Paper covers Rock.");
            var spockDefeatedByValue = DefeatedByValue.From(AttackValue.From("Spock", 3), "Spock vaporizes Rock.");
            var challange = ChallangeValue.From(rockAttack, new List<DefeatedByValue> { paperDefeatedByValue, spockDefeatedByValue });

            //action
            var challanges = appSettings.GetChallanges();

            //assert
            challanges.Count.Should().Be(1);
            challanges.Should().Contain(challange);
        }

        [Fact]
        public void GetAttacks_Throws_ArgumentOutOfRangeException_When_AttackType_Invalid()
        {
            //arrange
            var appSettings = Make_AppSettings("appsettings.brokenattacks.json");

            //action
            Action action = () => appSettings.GetChallanges();

            //assert
            action.Should()
                .Throw<InvalidDataException>()
                .WithMessage("Challanges configuration contains invalid attack values");
        }

        [Fact]
        public void GetAttacks_Throws_ArgumentOutOfRangeException_When_DefeatedBy_AttackType_Invalid()
        {
            //arrange
            var appSettings = Make_AppSettings("appsettings.brokendefeatedby.json");

            //action
            Action action = () => appSettings.GetChallanges();

            //assert
            action.Should()
                .Throw<InvalidDataException>()
                .WithMessage($"DefeatedBy configuration for [Rock] contains invalid attack values");
        }
    }
}