using FluentAssertions;
using Lakewood.AutoScale.Syntax;
using Xunit;

namespace Lakewood.AutoScale.UnitTests
{
    public class Parser_Tests
    {
        public static readonly object[] DoubleLiteralNodeTestCases = new object[]
        {
            new object[]
            {
                "1.0",
                new DoubleLiteralNode(1.0)
            },
        };

        [Theory]
        [MemberData(nameof(DoubleLiteralNodeTestCases))]
        public void Parser_RecognizesDoubleLiteralNode(string input, SyntaxNode expectedNode)
        {
            var parser = new Parser(input);

            SyntaxNode root = null;
            try
            {
                root = parser.Parse();
            }
            catch (ParseException ex)
            {
                ex.ExpectedTokenType.Should().Be(AutoScaleTokenType.DoubleLiteral);
            }

            root.Should().Be(expectedNode);
        }

        public static readonly object[] IdentifierNodeTestCases = new object[]
        {
            new object[]
            {
                "abc",
                new IdentifierNode("abc")
            },
        };

        [Theory]
        [MemberData(nameof(IdentifierNodeTestCases))]
        public void Parser_RecognizesIdentifierNode(string input, SyntaxNode expectedNode)
        {
            var parser = new Parser(input);

            SyntaxNode root = null;
            try
            {
                root = parser.Parse();
            }
            catch (ParseException ex)
            {
                ex.ExpectedTokenType.Should().Be(AutoScaleTokenType.Identifier);
            }

            root.Should().Be(expectedNode);
        }

        public static readonly object[] StringLiteralNodeTestCases = new object[]
        {
            new object[]
            {
                "\"1.0\"",
                new StringLiteralNode("\"1.0\"")
            },
        };

        [Theory]
        [MemberData(nameof(StringLiteralNodeTestCases))]
        public void Parser_RecognizesStringLiteralNode(string input, SyntaxNode expectedNode)
        {
            var parser = new Parser(input);

            SyntaxNode root = null;
            try
            {
                root = parser.Parse();
            }
            catch (ParseException ex)
            {
                ex.ExpectedTokenType.Should().Be(AutoScaleTokenType.StringLiteral);
            }

            root.Should().Be(expectedNode);
        }
    }
}
