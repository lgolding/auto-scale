using FluentAssertions;
using Lakewood.AutoScale.Syntax;
using Xunit;

namespace Lakewood.AutoScale.UnitTests
{
    public class Parser_Tests
    {
        public static readonly object[] ParserTestCases = new object[]
        {
            new object[]
            {
                "Empty formula",
                "",
                new FormulaNode()
            },

            new object[]
            {
                "DoubleLiteralNode",
                "1.0",
                new FormulaNode(
                    new DoubleLiteralNode(1.0))
            },

            new object[]
            {
                "IdentifierNode",
                "abc",
                new FormulaNode(
                    new IdentifierNode("abc"))
            },

            new object[]
            {
                "StringLiteralNode",
                "\"1.0\"",
                new FormulaNode(
                    new StringLiteralNode("\"1.0\""))
            },

            new object[]
            {
                "FormulaNode",
                "1.0;abc;\"1.0\"",
                new FormulaNode(
                    new DoubleLiteralNode(1.0),
                    new IdentifierNode("abc"),
                    new StringLiteralNode("\"1.0\""))
            },
        };

        [Theory]
        [MemberData(nameof(ParserTestCases))]
        public void Parser_RecognizesDoubleLiteralNode(string testName, string input, FormulaNode expectedNode)
        {
            var parser = new Parser(input);

            FormulaNode root = null;
            root = parser.Parse();

            root.Should().Be(expectedNode);
        }
    }
}
