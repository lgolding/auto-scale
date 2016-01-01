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

            new object[]
            {
                "Assign string literal",
                "a1 = \"1.0\";",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a1"),
                        new StringLiteralNode("\"1.0\"")))
            },

            new object[]
            {
                "Multiple assignments",
                "a1 = 1.0;\nabc=2",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a1"),
                        new DoubleLiteralNode(1.0)),
                    new AssignmentNode(
                        new IdentifierNode("abc"),
                        new DoubleLiteralNode(2.0)))
            },

            new object[]
            {
                "Assign identifier",
                "a1 = \"1.0\";\nb=a1",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a1"),
                        new StringLiteralNode("\"1.0\"")),
                    new AssignmentNode(
                        new IdentifierNode("b"),
                        new IdentifierNode("a1")))
            },

            new object[]
            {
                "Ternary expression",
                "a=1.0?\"x\":\"y\";",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new TernaryOperatorNode(
                            new DoubleLiteralNode(1.0),
                            new StringLiteralNode("\"x\""),
                            new StringLiteralNode("\"y\""))))
            },

            new object[]
            {
                "Ternary expression with white space",
                "a = 1.0 ? \"x\" : \"y\";",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new TernaryOperatorNode(
                            new DoubleLiteralNode(1.0),
                            new StringLiteralNode("\"x\""),
                            new StringLiteralNode("\"y\""))))
            },

            new object[]
            {
                "Logical OR expression",
                "a = 1.0 || b",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalOr,
                            new DoubleLiteralNode(1.0),
                            new IdentifierNode("b"))))
            },

            new object[]
            {
                "Logical OR expression within ternary expression",
                "a = 1.0 ? b || c : d || e",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new TernaryOperatorNode(
                            new DoubleLiteralNode(1.0),
                            new BinaryOperationNode(
                                BinaryOperator.LogicalOr,
                                new IdentifierNode("b"),
                                new IdentifierNode("c")),
                            new BinaryOperationNode(
                                BinaryOperator.LogicalOr,
                                new IdentifierNode("d"),
                                new IdentifierNode("e")))))
            },

            new object[]
            {
                "Logical OR expression as condition of ternary expression",
                "a = 1.0 || b ? c : d || e",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new TernaryOperatorNode(
                            new BinaryOperationNode(
                                BinaryOperator.LogicalOr,
                                new DoubleLiteralNode(1.0),
                                new IdentifierNode("b")),
                            new IdentifierNode("c"),
                            new BinaryOperationNode(
                                BinaryOperator.LogicalOr,
                                new IdentifierNode("d"),
                                new IdentifierNode("e")))))
            }
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
