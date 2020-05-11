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
        public void Updating_Tokens_Migrates_previous_Tokens_Active_State()
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

        [Fact]
        public void Updating_Tokens_Migrates_previous_Tokens_Selected_Value()
        {
            // Arrange
            var settings = new Settings();
            settings.MsBuildCommandLine = "/target:Build /target:Clean";
            settings.Tokens[0].SelectedElement = 1;
         
            // Act
          
            settings.MsBuildCommandLine = "/target:Build /target:Clean /p:Foo";

            // Assert
            settings.Tokens[0].SelectedElement.Should().Be(1);
        }

        [Fact]
        public void Updating_Tokens_Finds_Used_Element()
        {
            // Arrange
            var settings = new Settings();
            settings.MsBuildCommandLine = "/target:Build /target:Clean";
            settings.Tokens[0].SelectedElement = 1;

            // Act
            settings.MsBuildCommandLine = "/target:Build  /target:AAA /target:Clean";

            // Assert
            settings.Tokens[0].SelectedElement.Should().Be(2);
        }
    }
}
