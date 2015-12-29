using FluentAssertions;
using Lakewood.AutoScale.Syntax;
using Xunit;

namespace Lakewood.AutoScale.UnitTests
{
    public class Parser_Tests
    {
        public static readonly object[] LiteralNodeTestCases = new object[]
        {
            new object[]
            {
                "1.0",
                new DoubleLiteralNode(1.0)
            },

            new object[]
            {
                "x",
                null
            }
        };

        [Theory]
        [MemberData(nameof(LiteralNodeTestCases))]
        public void Parser_RecognizesLiteralNode(string input, SyntaxNode expectedNode)
        {
            var parser = new Parser(input);

            SyntaxNode root = null;
            try
            {
                root = parser.Parse();
            }
            catch (ParseException ex)
            {
                ex.ExpectedTokenType.Should().Be(AutoScaleTokenType.Literal);
            }

            root.Should().Be(expectedNode);
        }
    }
}
