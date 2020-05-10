using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MSBuildRunnerGUI.Data;
using Xunit;

namespace UnitTests
{
    [Trait("Category","Unit")]
    public class SettingsTests
    {
        [Fact]
        public void Updating_Tokens_Migrates_previous_Tokens()
        {
            // Arrange
            var settings = new Settings();
            settings.MsBuildCommandLine = "/target:Build";
            var originalActiveState = settings.Tokens[0].IsActive;
            var originalKey = settings.Tokens[0].TokenKey;

            // Act
            settings.Tokens[0].IsActive = !originalActiveState;
            settings.MsBuildCommandLine = "/target:Build /p:Foo";

            // Assert
            settings.Tokens.First(t => t.TokenKey == originalKey).IsActive.Should().Be(!originalActiveState);
        }
    }
}
