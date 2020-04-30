using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MSBuildRunnerGUI.Data;
using MSBuildRunnerGUI.Logic;
using Xunit;

namespace UnitTests
{
    [Trait("Category","Unit")]
    public class TokenParserTests
    {

        [Fact]
        public void Empty_String_Returns_Empty_List()
        {
            // Arrange

            // Act
            var list = TokenParser.Parse(string.Empty);

            // Assert
            list.Should().BeEmpty();
        }


        [Fact]
        public void Null_String_Throws()
        {
            // Arrange
            Action subject = () => TokenParser.Parse(null);

            // Act
            subject.Should().Throw<ArgumentNullException>();

        }

        [Fact]
        public void Parse_dashes()
        {
            // Arrange
            var str = "-a -b -c";

            // Act
            var list = TokenParser.Parse(str);

            // Assert
            list[0].Value.Should().Be("-a");
            list[1].Value.Should().Be("-b");
            list[2].Value.Should().Be("-c");
            list.Count.Should().Be(3);
        }


        [Fact]
        public void Parse_slashes()
        {
            // Arrange
            var str = "/a /b:32";

            // Act
            var list = TokenParser.Parse(str);

            // Assert
            list[0].Value.Should().Be("/a");
            list[1].Value.Should().Be("/b:32");
            list.Count.Should().Be(2);

        }


        [Fact]
        public void Parse_whitespaces_properly_with_double_quotes()
        {
            // Arrange
            var str = "\"one entry\"";

            // Act
            var list = TokenParser.Parse(str);

            // Assert
            list[0].Value.Should().Be("\"one entry\"");
            list.Count.Should().Be(1);
        }

        [Fact]
        public void Parse_whitespaces_properly_with_single_quotes()
        {
            // Arrange
            var str = "'one entry'";

            // Act
            var list = TokenParser.Parse(str);

            // Assert
            list[0].Value.Should().Be("'one entry'");
            list.Count.Should().Be(1);
        }


        [Fact]
        public void Parse_with_no_delimiter()
        {
            // Arrange
            var str = "a b \"c long\" d";

            // Act
            var list = TokenParser.Parse(str);

            // Assert
            list[0].Value.Should().Be("a");
            list[1].Value.Should().Be("b");
            list[2].Value.Should().Be("\"c long\"");
            list[3].Value.Should().Be("d");

            list.Count.Should().Be(4);

        }
    }
}
