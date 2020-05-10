using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MSBuildRunnerGUI.Contracts;
using MSBuildRunnerGUI.Logic;
using Xunit;

namespace UnitTests
{
    [Trait("Category", "Unit")]
    public class MsBuildRunnerTests
    {

        [Fact]
        public void Test_that_Fullpath_is_replaced()
        {
            // Arrange
            var exePath = "calc.exe";
            var commandLine = "%file%";
            var testPath = @"c:\foo\bar.project";

            var mockIo = new Mock<IFileIO>();
            mockIo.Setup(io => io.Exists(exePath)).Returns(true);


            var msbuild = new MsBuildRunner(mockIo.Object, exePath, commandLine);

            // Act
            msbuild.RunMsBuild(testPath, true);

            // Assert
            mockIo.Verify(m => m.WriteAllText(It.IsAny<string>(), It.Is<string>(s => s.Contains(testPath))));
        }

        [Fact]
        public void Test_that_Filename_is_replaced()
        {
            // Arrange
            var exePath = "calc.exe";
            var commandLine = "%filename% ";
            var testPath = @"c:\foo\bar.project";

            var mockIo = new Mock<IFileIO>();
            mockIo.Setup(io => io.Exists(exePath)).Returns(true);


            var msbuild = new MsBuildRunner(mockIo.Object, exePath, commandLine);

            // Act
            msbuild.RunMsBuild(testPath, true);

            // Assert
            mockIo.Verify(m => m.WriteAllText(It.IsAny<string>(), It.Is<string>(s => s.Contains(Path.GetFileNameWithoutExtension(testPath)))));
        }
    }
}
