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
            list[0].Values[0].Should().Be("-a");
            list[1].Values[0].Should().Be("-b");
            list[2].Values[0].Should().Be("-c");
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
            list[0].Values[0].Should().Be("/a");
            list[1].Values[0].Should().Be("/b:32");
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
            list[0].Values[0].Should().Be("\"one entry\"");
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
            list[0].Values[0].Should().Be("'one entry'");
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
            list[0].Values[0].Should().Be("a");
            list[1].Values[0].Should().Be("b");
            list[2].Values[0].Should().Be("\"c long\"");
            list[3].Values[0].Should().Be("d");

            list.Count.Should().Be(4);

        }


        [Fact]
        public void Parse_quoted_param()
        {
            // Arrange
            var str = "-a:\"c long\"";

            // Act
            var list = TokenParser.Parse(str);

            // Assert
            list[0].Values[0].Should().Be("-a:\"c long\"");


            list.Count.Should().Be(1);

        }

        public static IEnumerable<object[]> GroupedData =>
            new List<object[]>
            {
                new object[] {new []{"-target:Build","/target:Clean"} },
                new object[] {new []{ "/property:Configuration=Release","/property:Configuration=Debug" } },
                new object[] {new []{ "/p:Platform=x64","/p:Platform=x86" } },
                new object[] {new []{ "/verbosity:minimal","/verbosity:diag" } },
            };


        [MemberData(nameof(GroupedData))]
        [Theory]
        public void Parser_should_group_this(string[] lines)
        {
            // Arrange
            var argString = string.Join(" ", lines);

            // Act
            var list = TokenParser.Parse(argString);

            // Assert
            for(var i = 0; i < lines.Length; ++i)
            {
                list[0].Values[i] = lines[i];
            }

            list[0].Values.Count.Should().Be(lines.Length);
            list.Count.Should().Be(1);
        }

        public static IEnumerable<object[]> NotGroupedData =>
            new List<object[]>
            {
                new object[] {new []{ "/p:Platform=x64", "/p:Configuration=Release" } },
                new object[] {new []{ "/p:Foo", "/p:Bar" } },
            };


        [MemberData(nameof(NotGroupedData))]
        [Theory]
        public void Parser_should_never_group_this(string[] lines)
        {
            // Arrange
            var argString = string.Join(" ", lines);

            // Act
            var list = TokenParser.Parse(argString);

            // Assert
            for (var i = 0; i < lines.Length; ++i)
            {
                list[i].Values[0] = lines[i];
                list[i].Values.Count.Should().Be(1);
            }

            list.Count.Should().Be(lines.Length);
        }
    }
}
