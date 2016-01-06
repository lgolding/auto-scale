// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using FluentAssertions;
using Lakewood.AutoScale.Diagnostics;
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
                        SyntaxNodeFactory.MakeIdentifier("a1", 0),
                        SyntaxNodeFactory.MakeDoubleLiteral("1.0", 3)))
            },

            new object[]
            {
                "Assignment with white space",
                " a1 \t  = 1.0  ",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a1", 1),
                        SyntaxNodeFactory.MakeDoubleLiteral("1.0", 9)))
            },

            new object[]
            {
                "Assignment with semicolon",
                "a1 = 1.0;",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a1", 0),
                        SyntaxNodeFactory.MakeDoubleLiteral("1.0", 5)))
            },

            new object[]
            {
                "Assign string literal",
                "a1 = \"1.0\";",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a1", 0),
                        SyntaxNodeFactory.MakeStringLiteral("\"1.0\"", 5)))
            },

            new object[]
            {
                "Multiple assignments",
                "a1 = 1.0;\nabc=2",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a1", 0),
                        SyntaxNodeFactory.MakeDoubleLiteral("1.0", 5)),
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("abc", 10),
                        SyntaxNodeFactory.MakeDoubleLiteral("2", 14)))
            },

            new object[]
            {
                "Assign identifier",
                "a1 = \"1.0\";\nb=a1",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a1", 0),
                        SyntaxNodeFactory.MakeStringLiteral("\"1.0\"", 5)),
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("b", 12),
                        SyntaxNodeFactory.MakeIdentifier("a1", 14)))
            },

            new object[]
            {
                "Ternary expression",
                "a=1.0?\"x\":\"y\";",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new TernaryOperationNode(
                            SyntaxNodeFactory.MakeDoubleLiteral("1.0", 2),
                            SyntaxNodeFactory.MakeStringLiteral("\"x\"", 6),
                            SyntaxNodeFactory.MakeStringLiteral("\"y\"", 10))))
            },

            new object[]
            {
                "Ternary expression with white space",
                "a = 1.0 ? \"x\" : \"y\";",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new TernaryOperationNode(
                            SyntaxNodeFactory.MakeDoubleLiteral("1.0", 4),
                            SyntaxNodeFactory.MakeStringLiteral("\"x\"", 10),
                            SyntaxNodeFactory.MakeStringLiteral("\"y\"", 16))))
            },

            new object[]
            {
                "Logical OR expression",
                "a = 1.0 || b",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalOr,
                            SyntaxNodeFactory.MakeDoubleLiteral("1.0", 4),
                            SyntaxNodeFactory.MakeIdentifier("b", 11))))
            },

            new object[]
            {
                "Logical OR expression within ternary expression",
                "a = 1.0 ? b || c : d || e",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new TernaryOperationNode(
                            SyntaxNodeFactory.MakeDoubleLiteral("1.0", 4),
                            new BinaryOperationNode(
                                BinaryOperator.LogicalOr,
                                SyntaxNodeFactory.MakeIdentifier("b", 10),
                                SyntaxNodeFactory.MakeIdentifier("c", 15)),
                            new BinaryOperationNode(
                                BinaryOperator.LogicalOr,
                                SyntaxNodeFactory.MakeIdentifier("d", 19),
                                SyntaxNodeFactory.MakeIdentifier("e", 24)))))
            },

            new object[]
            {
                "Logical OR expression as condition of ternary expression",
                "a = 1.0 || b ? c : d || e",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new TernaryOperationNode(
                            new BinaryOperationNode(
                                BinaryOperator.LogicalOr,
                                SyntaxNodeFactory.MakeDoubleLiteral("1.0", 4),
                                SyntaxNodeFactory.MakeIdentifier("b", 11)),
                            SyntaxNodeFactory.MakeIdentifier("c", 15),
                            new BinaryOperationNode(
                                BinaryOperator.LogicalOr,
                                SyntaxNodeFactory.MakeIdentifier("d", 19),
                                SyntaxNodeFactory.MakeIdentifier("e", 24)))))
            },

            new object[]
            {
                "Logical AND expression",
                "a = 1.0 && b",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalAnd,
                            SyntaxNodeFactory.MakeDoubleLiteral("1.0", 4),
                            SyntaxNodeFactory.MakeIdentifier("b", 11))))
            },

            new object[]
            {
                "Logical AND with logical OR",
                "a = 1.0 && b || c",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalOr,
                            new BinaryOperationNode(
                                BinaryOperator.LogicalAnd,
                                SyntaxNodeFactory.MakeDoubleLiteral("1.0", 4),
                                SyntaxNodeFactory.MakeIdentifier("b", 11)),
                            SyntaxNodeFactory.MakeIdentifier("c", 16))))
            },

            new object[]
            {
                "Chained logical OR operations",
                "a = b || c || d",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalOr,
                            new BinaryOperationNode(
                                BinaryOperator.LogicalOr,
                                SyntaxNodeFactory.MakeIdentifier("b", 4),
                                SyntaxNodeFactory.MakeIdentifier("c", 9)),
                            SyntaxNodeFactory.MakeIdentifier("d", 14))))
            },

            new object[]
            {
                "Chained logical AND operations",
                "a = b && c && d",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalAnd,
                            new BinaryOperationNode(
                                BinaryOperator.LogicalAnd,
                                SyntaxNodeFactory.MakeIdentifier("b", 4),
                                SyntaxNodeFactory.MakeIdentifier("c", 9)),
                            SyntaxNodeFactory.MakeIdentifier("d", 14))))
            },

            new object[]
            {
                "Comparison expressions: Less/equal",
                "a = b < c && d <= e",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalAnd,
                            new BinaryOperationNode(
                                BinaryOperator.LessThan,
                                SyntaxNodeFactory.MakeIdentifier("b", 4),
                                SyntaxNodeFactory.MakeIdentifier("c", 8)),
                            new BinaryOperationNode(
                                BinaryOperator.LessThanOrEqual,
                                SyntaxNodeFactory.MakeIdentifier("d", 13),
                                SyntaxNodeFactory.MakeIdentifier("e", 18)))))
            },

            new object[]
            {
                "Comparison expressions: Greater/equal",
                "a = b > c || d >= e",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalOr,
                            new BinaryOperationNode(
                                BinaryOperator.GreaterThan,
                                SyntaxNodeFactory.MakeIdentifier("b", 4),
                                SyntaxNodeFactory.MakeIdentifier("c", 8)),
                            new BinaryOperationNode(
                                BinaryOperator.GreaterThanOrEqual,
                                SyntaxNodeFactory.MakeIdentifier("d", 13),
                                SyntaxNodeFactory.MakeIdentifier("e", 18)))))
            },

            new object[]
            {
                "Comparison expressions: Equal/not equal",
                "a = b == c && d != e",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalAnd,
                            new BinaryOperationNode(
                                BinaryOperator.Equality,
                                SyntaxNodeFactory.MakeIdentifier("b", 4),
                                SyntaxNodeFactory.MakeIdentifier("c", 9)),
                            new BinaryOperationNode(
                                BinaryOperator.NotEqual,
                                SyntaxNodeFactory.MakeIdentifier("d", 14),
                                SyntaxNodeFactory.MakeIdentifier("e", 19)))))
            },

            new object[]
            {
                "Unary operators",
                "a = -b > 0 && !e < 1",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.LogicalAnd,
                            new BinaryOperationNode(
                                BinaryOperator.GreaterThan,
                                new UnaryOperationNode(
                                    TokenFactory.MakeOperatorSubtraction(4),
                                    UnaryOperator.Negative,
                                    SyntaxNodeFactory.MakeIdentifier("b", 5)),
                                SyntaxNodeFactory.MakeDoubleLiteral("0", 9)),
                            new BinaryOperationNode(
                                BinaryOperator.LessThan,
                                new UnaryOperationNode(
                                    TokenFactory.MakeOperatorNot(14),
                                    UnaryOperator.LogicalNot,
                                    SyntaxNodeFactory.MakeIdentifier("e", 15)),
                                SyntaxNodeFactory.MakeDoubleLiteral("1", 19)))))
            },

            new object[]
            {
                "Nested unary operators",
                "a = - ! - b;",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new UnaryOperationNode(
                            TokenFactory.MakeOperatorSubtraction(4),
                            UnaryOperator.Negative,
                            new UnaryOperationNode(
                                TokenFactory.MakeOperatorNot(6),
                                UnaryOperator.LogicalNot,
                                new UnaryOperationNode(
                                    TokenFactory.MakeOperatorSubtraction(8),
                                    UnaryOperator.Negative,
                                    SyntaxNodeFactory.MakeIdentifier("b", 10))))))
            },

            new object[]
            {
                "Addition",
                "a = b + c;",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.Addition,
                            SyntaxNodeFactory.MakeIdentifier("b", 4),
                            SyntaxNodeFactory.MakeIdentifier("c", 8))))
            },

            new object[]
            {
                "Subtraction",
                "a = b - c;",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.Subtraction,
                            SyntaxNodeFactory.MakeIdentifier("b", 4),
                            SyntaxNodeFactory.MakeIdentifier("c", 8))))
            },

            new object[]
            {
                "Chained additive operators",
                "a = b - c + d;",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.Addition,
                            new BinaryOperationNode(
                                BinaryOperator.Subtraction,
                                SyntaxNodeFactory.MakeIdentifier("b", 4),
                                SyntaxNodeFactory.MakeIdentifier("c", 8)),
                            SyntaxNodeFactory.MakeIdentifier("d", 12))))
            },

            new object[]
            {
                "Additive operators mixed with unary operators",
                "a = -b --c",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.Subtraction,
                            new UnaryOperationNode(
                                TokenFactory.MakeOperatorSubtraction(4),
                                UnaryOperator.Negative,
                                SyntaxNodeFactory.MakeIdentifier("b", 5)),
                            new UnaryOperationNode(
                                TokenFactory.MakeOperatorSubtraction(8),
                                UnaryOperator.Negative,
                                SyntaxNodeFactory.MakeIdentifier("c", 9)))))
            },

            new object[]
            {
                "Multiplication",
                "a = b * c;",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.Multiplication,
                            SyntaxNodeFactory.MakeIdentifier("b", 4),
                            SyntaxNodeFactory.MakeIdentifier("c", 8))))
            },

            new object[]
            {
                "Division",
                "a = b / c;",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.Division,
                            SyntaxNodeFactory.MakeIdentifier("b", 4),
                            SyntaxNodeFactory.MakeIdentifier("c", 8))))
            },

            new object[]
            {
                "Chained multiplicative operators",
                "a = b * c / d;",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.Division,
                            new BinaryOperationNode(
                                BinaryOperator.Multiplication,
                                SyntaxNodeFactory.MakeIdentifier("b", 4),
                                SyntaxNodeFactory.MakeIdentifier("c", 8)),
                            SyntaxNodeFactory.MakeIdentifier("d", 12))))
            },

            new object[]
            {
                "Parenthesized expression",
                "a = (b + c) * d",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.Multiplication,
                            new ParenthesizedExpressionNode(
                                TokenFactory.MakeOpenParen(4),
                                new BinaryOperationNode(
                                    BinaryOperator.Addition,
                                    SyntaxNodeFactory.MakeIdentifier("b", 5),
                                    SyntaxNodeFactory.MakeIdentifier("c", 9)),
                                TokenFactory.MakeCloseParen(10)),
                            SyntaxNodeFactory.MakeIdentifier("d", 14))))
            },

            new object[]
            {
                "Nested parentheses",
                "a = (b * (c + d))",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new ParenthesizedExpressionNode(
                            TokenFactory.MakeOpenParen(4),
                            new BinaryOperationNode(
                                BinaryOperator.Multiplication,
                                SyntaxNodeFactory.MakeIdentifier("b", 5),
                                new ParenthesizedExpressionNode(
                                    TokenFactory.MakeOpenParen(9),
                                    new BinaryOperationNode(
                                        BinaryOperator.Addition,
                                        SyntaxNodeFactory.MakeIdentifier("c", 10),
                                        SyntaxNodeFactory.MakeIdentifier("d", 14)),
                                    TokenFactory.MakeCloseParen(15))),
                            TokenFactory.MakeCloseParen(16))))
            },

            new object[]
            {
                "Function without arguments",
                "a = rand()",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new FunctionCallNode(
                            SyntaxNodeFactory.MakeIdentifier("rand", 4),
                            new SyntaxNode[0],
                            TokenFactory.MakeCloseParen(9))))
            },

            new object[]
            {
                "Function with one argument",
                "a = lg(1.0)",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new FunctionCallNode(
                            SyntaxNodeFactory.MakeIdentifier("lg", 4),
                            new SyntaxNode[]
                            {
                                SyntaxNodeFactory.MakeDoubleLiteral("1.0", 7)
                            },
                            TokenFactory.MakeCloseParen(10))))
            },

            new object[]
            {
                "Functon with multiple arguments",
                "a = percentile(v, 90.0)",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new FunctionCallNode(
                            SyntaxNodeFactory.MakeIdentifier("percentile", 4),
                            new SyntaxNode[]
                            {
                                SyntaxNodeFactory.MakeIdentifier("v", 15),
                                SyntaxNodeFactory.MakeDoubleLiteral("90.0", 18)
                            },
                            TokenFactory.MakeCloseParen(22))))
            },

            new object[]
            {
                "Function with multiple complex arguments",
                "a = percentile(v * 3, min(p, 90.0) + 5)",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new FunctionCallNode(
                            SyntaxNodeFactory.MakeIdentifier("percentile", 4),
                            new SyntaxNode[]
                            {
                                new BinaryOperationNode(
                                    BinaryOperator.Multiplication,
                                    SyntaxNodeFactory.MakeIdentifier("v", 15),
                                    SyntaxNodeFactory.MakeDoubleLiteral("3", 19)),
                                new BinaryOperationNode(
                                    BinaryOperator.Addition,
                                    new FunctionCallNode(
                                        SyntaxNodeFactory.MakeIdentifier("min", 22),
                                        new SyntaxNode[]
                                        {
                                            SyntaxNodeFactory.MakeIdentifier("p", 26),
                                            SyntaxNodeFactory.MakeDoubleLiteral("90.0", 29)
                                        },
                                        TokenFactory.MakeCloseParen(33)),
                                    SyntaxNodeFactory.MakeDoubleLiteral("5", 37))
                            },
                            TokenFactory.MakeCloseParen(38))))
            },

            new object[]
            {
                "Method invocation",
                "a = $CPUPercent.GetSample(start, end) * 2",
                new FormulaNode(
                    new AssignmentNode(
                        SyntaxNodeFactory.MakeIdentifier("a", 0),
                        new BinaryOperationNode(
                            BinaryOperator.Multiplication,
                            new MethodInvocationNode(
                                SyntaxNodeFactory.MakeIdentifier("$CPUPercent", 4),
                                SyntaxNodeFactory.MakeIdentifier("GetSample", 16),
                                new SyntaxNode[]
                                {
                                    SyntaxNodeFactory.MakeIdentifier("start", 26),
                                    SyntaxNodeFactory.MakeIdentifier("end", 33)
                                },
                                TokenFactory.MakeCloseParen(36)),
                            SyntaxNodeFactory.MakeDoubleLiteral("2", 40))))
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
                    new Diagnostic(
                        ParserError.Descriptor,
                        ParserError.UnexpectedTokenMessage(
                            TokenFactory.MakeUnknownToken("^", 0),
                            AutoScaleTokenType.Identifier),
                        0, 0)
                }
            },

            new object[]
            {
                "Unknown token instead of assignment operator",
                "a^1+2",
                new []
                {
                    new Diagnostic(
                        ParserError.Descriptor,
                        ParserError.UnexpectedTokenMessage(
                            TokenFactory.MakeUnknownToken("^", 1),
                            AutoScaleTokenType.OperatorAssign),
                        1, 1)
                }
            },

            new object[]
            {
                "Unknown token instead of expression",
                "a=^",
                new []
                {
                    new Diagnostic(
                        ParserError.Descriptor,
                        ParserError.UnexpectedTokenMessage(
                            TokenFactory.MakeUnknownToken("^", 2),
                            AutoScaleTokenType.DoubleLiteral,
                            AutoScaleTokenType.StringLiteral,
                            AutoScaleTokenType.Identifier),
                        2, 2)
                }
            },

            new object[]
            {
                "Reports only one parse error per statement",
                "^=^",
                new []
                {
                    new Diagnostic(
                        ParserError.Descriptor,
                        ParserError.UnexpectedTokenMessage(
                            TokenFactory.MakeUnknownToken("^", 0),
                            AutoScaleTokenType.Identifier),
                        0, 0)
                }
            },

            new object[]
            {
                "Reports parse errors in multiple statements",
                "^=^;^=1",
                new []
                {
                    new Diagnostic(
                        ParserError.Descriptor,
                        ParserError.UnexpectedTokenMessage(
                            TokenFactory.MakeUnknownToken("^", 0),
                            AutoScaleTokenType.Identifier),
                        0, 0),
                    new Diagnostic(
                        ParserError.Descriptor,
                        ParserError.UnexpectedTokenMessage(
                            TokenFactory.MakeUnknownToken("^", 4),
                            AutoScaleTokenType.Identifier),
                        4, 4)
                }
            },

            new object[]
            {
                "#8: Don't report parse error for comment",
                "// Comment\na = 1;",
                new Diagnostic[0]
            },

            new object[]
            {
                "#9: Don't report parse error for trailing white space after error",
                "^ = 1;\n",
                new []
                {
                    new Diagnostic(
                        ParserError.Descriptor,
                        ParserError.UnexpectedTokenMessage(
                            TokenFactory.MakeUnknownToken("^", 0),
                            AutoScaleTokenType.Identifier),
                        0, 0)
                }
            },

            new object[]
            {
                "#17: Handle unclosed function call",
                "a = avg(",
                new []
                {
                    new Diagnostic(
                        ParserError.Descriptor,
                        Resources.ErrorUnexpectedEndOfFile,
                        8, 8)
                }
            },

            new object[]
            {
                "#17: Handle unclosed method invocation",
                "a = $ActiveTasks.GetSample(",
                new []
                {
                    new Diagnostic(
                        ParserError.Descriptor,
                        Resources.ErrorUnexpectedEndOfFile,
                        27, 27)
                }
            }
        };

        [Theory]
        [MemberData(nameof(ParserErrorTestCases))]
        public void Parser_produces_expected_diagnostics(string testName, string input, Diagnostic[] expectedDiagnostics)
        {
            var parser = new Parser(input);

            parser.Parse();

            parser.Diagnostics.Count.Should().Be(expectedDiagnostics.Length);
            parser.Diagnostics.Should().ContainInOrder(expectedDiagnostics);
        }
    }
}
