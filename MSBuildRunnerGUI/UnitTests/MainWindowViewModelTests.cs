using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MSBuildRunnerGUI;
using MSBuildRunnerGUI.Contracts;
using MSBuildRunnerGUI.Logic;
using MSBuildRunnerGUI.Persistence;
using Xunit;

namespace UnitTests
{
    [Trait("Category","Unit")]
    public class MainWindowViewModelTests
    {
        private MainWindowViewModel _sut;
        private readonly Mock<IFileIO> _fileMock;
        private readonly Mock<IUserSettingsManager> _userSettingsManagerMock;

        public MainWindowViewModelTests()
        {
            _fileMock = new Mock<IFileIO>();
            _userSettingsManagerMock = new Mock<IUserSettingsManager>();
            _userSettingsManagerMock.Setup(u => u.LoadFromLocation()).Returns(new UserSettings());
            SetSut();
        }

        private void SetSut()
        {
            _sut = new MainWindowViewModel(_fileMock.Object, _userSettingsManagerMock.Object);
        }

        [Fact]
        public void User_Settings_are_checked_for_consistence_when_tokens_mismatch()
        {
            // Arrange
            var persistedSettings = new UserSettings();
            persistedSettings.MsBuildCommandLine = "cmd /t:line /t:bar";
            persistedSettings.MsBuildExePath = "exe path";
            persistedSettings.LastSetDirectory = "last path";
            persistedSettings.ActiveStates = new List<bool> { false, false };
            persistedSettings.TokenPositions = new List<int> { 0, 1,2,3,4,5,6 };
            _userSettingsManagerMock.Setup(u => u.LoadFromLocation()).Returns(persistedSettings);

            // Act
            Action a = SetSut;

            // Assert
            a.Should().Throw<InvalidOperationException>().WithMessage($"*{persistedSettings.TokenPositions.Count}*");
        }

        [Fact]
        public void User_Settings_are_checked_for_consistence_when_active_states_mismatch()
        {
            // Arrange
            var persistedSettings = new UserSettings();
            persistedSettings.MsBuildCommandLine = "cmd /t:line /t:bar";
            persistedSettings.MsBuildExePath = "exe path";
            persistedSettings.LastSetDirectory = "last path";
            persistedSettings.ActiveStates = new List<bool> { false, false, true, true };
            persistedSettings.TokenPositions = new List<int> { 0, 1,  };
            _userSettingsManagerMock.Setup(u => u.LoadFromLocation()).Returns(persistedSettings);

            // Act
            Action a = SetSut;

            // Assert
            a.Should().Throw<InvalidOperationException>().WithMessage($"*{persistedSettings.ActiveStates.Count}*");
        }


        [Fact]
        public void Loading_Token_Positions_Checks_for_valid_position()
        {
            // Arrange
            var persistedSettings = new UserSettings();
            persistedSettings.MsBuildCommandLine = "cmd";
            persistedSettings.MsBuildExePath = "exe path";
            persistedSettings.LastSetDirectory = "last path";
            persistedSettings.ActiveStates = new List<bool> { false};
            persistedSettings.TokenPositions = new List<int> { 1 };
            _userSettingsManagerMock.Setup(u => u.LoadFromLocation()).Returns(persistedSettings);

            // Act
            Action a = SetSut;

            // Assert
            a.Should().Throw<InvalidOperationException>().WithMessage("*1*").WithMessage("*0*");
        }



        [Fact]
        public void UserSettings_are_loaded_on_start()
        {
            // Arrange
            var persistedSettings = new UserSettings();
            persistedSettings.MsBuildCommandLine = "cmd /t:line /t:bar";
            persistedSettings.MsBuildExePath = "exe path";
            persistedSettings.LastSetDirectory = "last path";
            persistedSettings.ActiveStates = new List<bool>{false, false};
            persistedSettings.TokenPositions = new List<int>{0, 1};
            _userSettingsManagerMock.Setup(u => u.LoadFromLocation()).Returns(persistedSettings);

            // Act
            SetSut();

            // Assert
            _userSettingsManagerMock.Verify(usm => usm.LoadFromLocation());

            _sut.PathToDirectory.Should().Be(persistedSettings.LastSetDirectory);
            _sut.Settings.MsBuildCommandLine.Should().Be(persistedSettings.MsBuildCommandLine);
            _sut.Settings.MsBuildPath.Should().Be(persistedSettings.MsBuildExePath);
            _sut.Settings.Tokens[0].IsActive.Should().BeFalse();
            _sut.Settings.Tokens[1].IsActive.Should().BeFalse();

            _sut.Settings.Tokens[0].SelectedElement.Should().Be(0);
            _sut.Settings.Tokens[1].SelectedElement.Should().Be(1);
        }


       
        [Fact]
        public void UserSettings_are_persisted_on_settings_change()
        {
            // Arrange
          
            UserSettings passedSettings = null;
            _userSettingsManagerMock.Setup(u => u.SaveSettings(It.IsAny<UserSettings>())).Callback<UserSettings>(settings => passedSettings = settings);

            // Act
            _sut.Settings.MsBuildCommandLine = "dummy /t:Clean /t:Foo";
            _sut.Settings.MsBuildPath = "some path";
            _sut.Settings.Tokens[0].IsActive = false;
            _sut.Settings.Tokens[1].SelectedElement = 1;

            // Assert
            passedSettings.Should().NotBeNull();
            
            _userSettingsManagerMock.Verify(usm => usm.SaveSettings(It.IsAny<UserSettings>()));
            
            passedSettings.MsBuildCommandLine.Should().Be(_sut.Settings.MsBuildCommandLine);
            passedSettings.MsBuildExePath.Should().Be(_sut.Settings.MsBuildPath);

            passedSettings.ActiveStates[0].Should().Be(_sut.Settings.Tokens[0].IsActive);
            passedSettings.ActiveStates.Count.Should().Be(2);
            passedSettings.TokenPositions.Should().ContainInOrder(new [] {0, 1});

        }


        [Fact]
        public void UserSettings_are_persisted_on_view_model_changes()
        {
            // Arrange
            UserSettings passedSettings = null;
            _userSettingsManagerMock.Setup(u => u.SaveSettings(It.IsAny<UserSettings>())).Callback<UserSettings>(settings => passedSettings = settings);

            // Act
            _sut.PathToDirectory = "New Path";

            // Assert
            passedSettings.Should().NotBeNull();

            _userSettingsManagerMock.Verify(usm => usm.SaveSettings(It.IsAny<UserSettings>()));
            passedSettings.LastSetDirectory.Should().Be(_sut.PathToDirectory);

        }


        [Fact]
        public void Change_of_settings_updates_tokens()
        {
            // Arrange
            UserSettings passedSettings = null;
            _userSettingsManagerMock.Setup(u => u.SaveSettings(It.IsAny<UserSettings>())).Callback<UserSettings>(settings => passedSettings = settings);
            
            _sut.Settings.MsBuildCommandLine = "dummy /t:Clean /t:Foo";

            // Act
            _sut.Settings.MsBuildCommandLine = "empty";

            // Assert
            passedSettings.Should().NotBeNull();

            _userSettingsManagerMock.Verify(usm => usm.SaveSettings(It.IsAny<UserSettings>()));
            passedSettings.MsBuildCommandLine.Should().Be(_sut.Settings.MsBuildCommandLine);
        }
    }
}
