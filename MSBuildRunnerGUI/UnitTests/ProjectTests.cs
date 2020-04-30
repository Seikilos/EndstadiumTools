using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MSBuildRunnerGUI.Contracts;
using MSBuildRunnerGUI.Data;
using UnitTests.Data;
using Xunit;

namespace UnitTests
{
    [Trait("Category","Unit")]
    public class ProjectTests
    {
        private Mock<IFileIO> _mockIO;

        public ProjectTests()
        {
            _mockIO = new Mock<IFileIO>();

        }

        [Fact]
        public async Task Dependencies_are_correctly_calculated_even_with_relative_paths()
        {
            var data = new CsProjectData();
            data
                .AddProjectReference("foo.csproj")
                .AddProjectReference(@".\foo.csproj");

            _mockIO.Setup(i => i.ReadFile("fullpath")).Returns(data.ToString);

            var sut = new Project("fullpath", _mockIO.Object);

            await WaitHelper.Wait(() => sut.ScanCompleted).ConfigureAwait(false);

            sut.TotalDependencies.Should().Be(1);



        }
    }
}
