﻿using FluentAssertions;
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
                "All white space",
                "   \t \n ",
                new FormulaNode()
            },

            new object[]
            {
                "Assign double literal",
                "a1=1.0",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a1"),
                        new DoubleLiteralNode(1.0)))
            },

            new object[]
            {
                "Assignment with white space",
                " a1 \t  = 1.0  ",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a1"),
                        new DoubleLiteralNode(1.0)))
            },

            new object[]
            {
                "Assignment with semicolon",
                "a1 = 1.0;",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a1"),
                        new DoubleLiteralNode(1.0)))
            },
        };

        [Theory]
        [MemberData(nameof(ParserTestCases))]
        public void Parser_ProducesExpectedFormula(string testName, string input, FormulaNode expectedNode)
        {
            var parser = new Parser(input);

            FormulaNode root = null;
            root = parser.Parse();

            root.Should().Be(expectedNode);
        }
    }
}
