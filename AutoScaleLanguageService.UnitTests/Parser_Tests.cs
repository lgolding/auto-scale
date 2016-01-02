using System.Linq;
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
                        new TernaryOperationNode(
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
                        new TernaryOperationNode(
                            new DoubleLiteralNode(1.0),
                            new StringLiteralNode("\"x\""),
                            new StringLiteralNode("\"y\""))))
            },

            // TODO: Chained operations (a || b || c...)
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
                        new TernaryOperationNode(
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
                        new TernaryOperationNode(
                            new BinaryOperationNode(
                                BinaryOperator.LogicalOr,
                                new DoubleLiteralNode(1.0),
                                new IdentifierNode("b")),
                            new IdentifierNode("c"),
                            new BinaryOperationNode(
                                BinaryOperator.LogicalOr,
                                new IdentifierNode("d"),
                                new IdentifierNode("e")))))
            },

            new object[]
            {
                "Logical AND expression",
                "a = 1.0 && b",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalAnd,
                            new DoubleLiteralNode(1.0),
                            new IdentifierNode("b"))))
            },

            new object[]
            {
                "Logical AND with logical OR",
                "a = 1.0 && b || c",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalOr,
                            new BinaryOperationNode(
                                BinaryOperator.LogicalAnd,
                                new DoubleLiteralNode(1.0),
                                new IdentifierNode("b")),
                            new IdentifierNode("c"))))
            },

            new object[]
            {
                "Chained logical OR operations",
                "a = b || c || d",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalOr,
                            new BinaryOperationNode(
                                BinaryOperator.LogicalOr,
                                new IdentifierNode("b"),
                                new IdentifierNode("c")),
                            new IdentifierNode("d"))))
            },

            new object[]
            {
                "Chained logical AND operations",
                "a = b && c && d",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalAnd,
                            new BinaryOperationNode(
                                BinaryOperator.LogicalAnd,
                                new IdentifierNode("b"),
                                new IdentifierNode("c")),
                            new IdentifierNode("d"))))
            },

            new object[]
            {
                "Comparison expressions: Less/equal",
                "a = b < c && d <= e",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalAnd,
                            new BinaryOperationNode(
                                BinaryOperator.LessThan,
                                new IdentifierNode("b"),
                                new IdentifierNode("c")),
                            new BinaryOperationNode(
                                BinaryOperator.LessThanOrEqual,
                                new IdentifierNode("d"),
                                new IdentifierNode("e")))))
            },

            new object[]
            {
                "Comparison expressions: Greater/equal",
                "a = b > c || d >= e",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalOr,
                            new BinaryOperationNode(
                                BinaryOperator.GreaterThan,
                                new IdentifierNode("b"),
                                new IdentifierNode("c")),
                            new BinaryOperationNode(
                                BinaryOperator.GreaterThanOrEqual,
                                new IdentifierNode("d"),
                                new IdentifierNode("e")))))
            },

            new object[]
            {
                "Comparison expressions: Equal/not equal",
                "a = b == c && d != e",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalAnd,
                            new BinaryOperationNode(
                                BinaryOperator.Equality,
                                new IdentifierNode("b"),
                                new IdentifierNode("c")),
                            new BinaryOperationNode(
                                BinaryOperator.NotEqual,
                                new IdentifierNode("d"),
                                new IdentifierNode("e")))))
            },

            new object[]
            {
                "Unary operators",
                "a = -b > 0 && !e < 1",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalAnd,
                            new BinaryOperationNode(
                                BinaryOperator.GreaterThan,
                                new UnaryOperationNode(
                                    UnaryOperator.Negative,
                                    new IdentifierNode("b")),
                                new DoubleLiteralNode(0.0)),
                            new BinaryOperationNode(
                                BinaryOperator.LessThan,
                                new UnaryOperationNode(
                                    UnaryOperator.LogicalNot,
                                    new IdentifierNode("e")),
                                new DoubleLiteralNode(1.0)))))
            },

            new object[]
            {
                "Nested unary operators",
                "a = - ! - b;",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new UnaryOperationNode(
                            UnaryOperator.Negative,
                            new UnaryOperationNode(
                                UnaryOperator.LogicalNot,
                                new UnaryOperationNode(
                                    UnaryOperator.Negative,
                                    new IdentifierNode("b"))))))
            },

            new object[]
            {
                "Addition",
                "a = b + c;",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.Addition,
                            new IdentifierNode("b"),
                            new IdentifierNode("c"))))
            },

            new object[]
            {
                "Subtraction",
                "a = b - c;",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.Subtraction,
                            new IdentifierNode("b"),
                            new IdentifierNode("c"))))
            },

            new object[]
            {
                "Chained additive operators",
                "a = b - c + d;",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.Addition,
                            new BinaryOperationNode(
                                BinaryOperator.Subtraction,
                                new IdentifierNode("b"),
                                new IdentifierNode("c")),
                            new IdentifierNode("d"))))
            },

            new object[]
            {
                "Additive operators mixed with unary operators",
                "a = -b --c",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.Subtraction,
                            new UnaryOperationNode(
                                UnaryOperator.Negative,
                                new IdentifierNode("b")),
                            new UnaryOperationNode(
                                UnaryOperator.Negative,
                                new IdentifierNode("c")))))
            },

            new object[]
            {
                "Multiplication",
                "a = b * c;",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.Multiplication,
                            new IdentifierNode("b"),
                            new IdentifierNode("c"))))
            },

            new object[]
            {
                "Division",
                "a = b / c;",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.Division,
                            new IdentifierNode("b"),
                            new IdentifierNode("c"))))
            },

            new object[]
            {
                "Chained multiplicative operators",
                "a = b * c / d;",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.Division,
                            new BinaryOperationNode(
                                BinaryOperator.Multiplication,
                                new IdentifierNode("b"),
                                new IdentifierNode("c")),
                            new IdentifierNode("d"))))
            },

            new object[]
            {
                "Parenthesized expression",
                "a = (b + c) * d",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.Multiplication,
                            new ParenthesizedExpressionNode(
                                new BinaryOperationNode(
                                    BinaryOperator.Addition,
                                    new IdentifierNode("b"),
                                    new IdentifierNode("c"))),
                            new IdentifierNode("d"))))
            },

            new object[]
            {
                "Nested parentheses",
                "a = (b * (c + d))",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new ParenthesizedExpressionNode(
                            new BinaryOperationNode(
                                BinaryOperator.Multiplication,
                                new IdentifierNode("b"),
                                new ParenthesizedExpressionNode(
                                    new BinaryOperationNode(
                                        BinaryOperator.Addition,
                                        new IdentifierNode("c"),
                                        new IdentifierNode("d")))))))
            },

            new object[]
            {
                "Function without arguments",
                "a = rand()",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new FunctionCallNode(
                            new IdentifierNode("rand"),
                            new SyntaxNode[0])))
            },

            new object[]
            {
                "Function with one argument",
                "a = lg(1.0)",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new FunctionCallNode(
                            new IdentifierNode("lg"),
                            new SyntaxNode[]
                            {
                                new DoubleLiteralNode(1.0)
                            })))
            },

            new object[]
            {
                "Functon with multiple arguments",
                "a = percentile(v, 90.0)",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new FunctionCallNode(
                            new IdentifierNode("percentile"),
                            new SyntaxNode[]
                            {
                                new IdentifierNode("v"),
                                new DoubleLiteralNode(90.0)
                            })))
            },

            new object[]
            {
                "Function with multiple complex arguments",
                "a = percentile(v * 3, min(p, 90.0) + 5)",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new FunctionCallNode(
                            new IdentifierNode("percentile"),
                            new SyntaxNode[]
                            {
                                new BinaryOperationNode(
                                    BinaryOperator.Multiplication,
                                    new IdentifierNode("v"),
                                    new DoubleLiteralNode(3)),
                                new BinaryOperationNode(
                                    BinaryOperator.Addition,
                                    new FunctionCallNode(
                                        new IdentifierNode("min"),
                                        new SyntaxNode[]
                                        {
                                            new IdentifierNode("p"),
                                            new DoubleLiteralNode(90.0)
                                        }),
                                    new DoubleLiteralNode(5.0))
                            })))
            },

            new object[]
            {
                "Method invocation",
                "a = $CPUPercent.GetSample(start, end) * 2",
                new FormulaNode(
                    new AssignmentNode(
                        new IdentifierNode("a"),
                        new BinaryOperationNode(
                            BinaryOperator.Multiplication,
                            new MethodInvocationNode(
                                new IdentifierNode("$CPUPercent"),
                                new IdentifierNode("GetSample"),
                                new SyntaxNode[]
                                {
                                    new IdentifierNode("start"),
                                    new IdentifierNode("end")
                                }),
                            new DoubleLiteralNode(2.0))))
            }
        };

        [Theory]
        [MemberData(nameof(ParserTestCases))]
        public void Parser_produces_expected_tree(string testName, string input, FormulaNode expectedNode)
        {
            var parser = new Parser(input);

            FormulaNode root = null;
            root = parser.Parse();

            root.Should().Be(expectedNode);
        }

        public static readonly object[] ParserErrorTestCases = new object[]
        {
            new object[]
            {
                "Unknown token instead of identifier",
                "^=1+2",
                new []
                {
                    "ASF0001"
                }
            },

            new object[]
            {
                "Unknown token instead of assignment operator",
                "a^1+2",
                new []
                {
                    "ASF0001"
                }
            },

            new object[]
            {
                "Unknown token instead of expression",
                "a=^",
                new []
                {
                    "ASF0001"
                }
            },

            new object[]
            {
                "Reports only one parse error per statement",
                "^=^",
                new object[]
                {
                    "ASF0001"
                }
            },

            new object[]
            {
                "Reports parse errors in multiple statements",
                "^=^;^=1",
                new object[]
                {
                    "ASF0001",
                    "ASF0001"
                }
            },
        };

        [Theory]
        [MemberData(nameof(ParserErrorTestCases))]
        public void Parser_produces_expected_errors(string testName, string input, string[] expectedErrors)
        {
            var parser = new Parser(input);

            parser.Parse();

            parser.Errors.Should().ContainInOrder(expectedErrors);
        }
    }
}
