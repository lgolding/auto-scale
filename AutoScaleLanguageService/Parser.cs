using System.Collections.Generic;
using System.Globalization;
using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale
{
    internal class Parser
    {
        private readonly Lexer _lexer;
        private List<string> _errors = new List<string>();

        internal Parser(string input)
        {
            _lexer = new Lexer(input);
        }

        internal FormulaNode Parse()
        {
            var assignments = new List<AssignmentNode>();

            _lexer.SkipWhite();
            while (_lexer.More())
            {
                assignments.Add(Assignment());

                _lexer.SkipWhite();
                if (_lexer.More())
                {
                    _lexer.Consume(AutoScaleTokenType.Semicolon);
                    _lexer.SkipWhite();
                }
            }

            return new FormulaNode(assignments.ToArray());
        }

        internal AssignmentNode Assignment()
        {
            IdentifierNode identifier = Identifier();

            _lexer.Consume(AutoScaleTokenType.OperatorAssign);

            SyntaxNode expression = Expression();

            return new AssignmentNode(identifier, expression);
        }

        private SyntaxNode Expression()
        {
            var condition = LogicalOrExpression();

            _lexer.SkipWhite();
            if (_lexer.Peek().Type == AutoScaleTokenType.OperatorTernaryQuestion)
            {
                _lexer.Skip();

                var trueValue = LogicalOrExpression();

                _lexer.Consume(AutoScaleTokenType.OperatorTernaryColon);

                var falseValue = LogicalOrExpression();

                return new TernaryOperatorNode(condition, trueValue, falseValue);
            }
            else
            {
                return condition;
            }
        }

        private SyntaxNode LogicalOrExpression()
        {
            var left = LogicalAndExpression();

            while (_lexer.More())
            {
                _lexer.SkipWhite();
                if (_lexer.Peek().Type == AutoScaleTokenType.OperatorLogicalOr)
                {
                    _lexer.Skip();
                    var right = LogicalAndExpression();

                    left = new BinaryOperationNode(BinaryOperator.LogicalOr, left, right);
                }
                else
                {
                    break;
                }
            }

            return left;
        }

        private SyntaxNode LogicalAndExpression()
        {
            var left = ComparisonExpression();

            while (_lexer.More())
            {
                _lexer.SkipWhite();
                if (_lexer.Peek().Type == AutoScaleTokenType.OperatorLogicalAnd)
                {
                    _lexer.Skip();
                    var right = ComparisonExpression();

                    left = new BinaryOperationNode(BinaryOperator.LogicalAnd, left, right);
                }
                else
                {
                    break;
                }
            }

            return left;
        }

        private SyntaxNode ComparisonExpression()
        {
            var left = UnaryExpression();

            while (_lexer.More())
            {
                _lexer.SkipWhite();

                var nextTokenType = _lexer.Peek().Type;
                BinaryOperator comparisonOperator = ComparisonOperatorFromTokenType(nextTokenType);
                if (comparisonOperator != BinaryOperator.Unknown)
                {
                    _lexer.Skip();
                    var right = UnaryExpression();

                    left = new BinaryOperationNode(comparisonOperator, left, right);
                }
                else
                {
                    break;
                }
            }

            return left;
        }

        private static readonly IDictionary<AutoScaleTokenType, BinaryOperator> s_comparisonOperatorDictionary = new Dictionary<AutoScaleTokenType, BinaryOperator>
        {
            [AutoScaleTokenType.OperatorLessThan] = BinaryOperator.LessThan,
            [AutoScaleTokenType.OperatorLessThanOrEqual] = BinaryOperator.LessThanOrEqual,
            [AutoScaleTokenType.OperatorEquality] = BinaryOperator.Equality,
            [AutoScaleTokenType.OperatorNotEqual] = BinaryOperator.NotEqual,
            [AutoScaleTokenType.OperatorGreaterThan] = BinaryOperator.GreaterThan,
            [AutoScaleTokenType.OperatorGreaterThanOrEqual] = BinaryOperator.GreaterThanOrEqual
        };

        private BinaryOperator ComparisonOperatorFromTokenType(AutoScaleTokenType tokenType)
        {
            BinaryOperator comparisonOperator;
            if (!s_comparisonOperatorDictionary.TryGetValue(tokenType, out comparisonOperator))
            {
                comparisonOperator = BinaryOperator.Unknown;
            }

            return comparisonOperator;
        }

        private SyntaxNode UnaryExpression()
        {
            _lexer.SkipWhite();

            AutoScaleTokenType nextTokenType = _lexer.Peek().Type;
            UnaryOperator unaryOperator = UnaryOperatorFromTokenType(nextTokenType);
            if (unaryOperator != UnaryOperator.Unknown)
            {
                _lexer.Skip();
                var primaryExpression = PrimaryExpression();
                return new UnaryOperationNode(unaryOperator, primaryExpression);
            }
            else
            {
                return PrimaryExpression();
            }
        }

        private static readonly IDictionary<AutoScaleTokenType, UnaryOperator> s_unaryOperatorDictionary = new Dictionary<AutoScaleTokenType, UnaryOperator>
        {
            [AutoScaleTokenType.OperatorNot] = UnaryOperator.LogicalNot,
            [AutoScaleTokenType.OperatorSubtraction] = UnaryOperator.Negative
        };

        private UnaryOperator UnaryOperatorFromTokenType(AutoScaleTokenType tokenType)
        {
            UnaryOperator unaryOperator;
            if (!s_unaryOperatorDictionary.TryGetValue(tokenType, out unaryOperator))
            {
                unaryOperator = UnaryOperator.Unknown;
            }

            return unaryOperator;
        }

        private SyntaxNode PrimaryExpression()
        {
            _lexer.SkipWhite();

            AutoScaleToken nextToken = _lexer.Peek();
            switch (nextToken.Type)
            {
                case AutoScaleTokenType.DoubleLiteral:
                    return DoubleLiteral();

                case AutoScaleTokenType.StringLiteral:
                    return StringLiteral();

                case AutoScaleTokenType.Identifier:
                    return Identifier();

                default:
                    throw new ParseException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.ErrorUnexpectedTokenWithChoices,
                            string.Join(", ",
                                AutoScaleTokenType.DoubleLiteral,
                                AutoScaleTokenType.StringLiteral,
                                AutoScaleTokenType.Identifier),
                            nextToken.Text,
                            nextToken.Type));
            }
        }

        internal IdentifierNode Identifier()
        {
            AutoScaleToken token = _lexer.Consume(AutoScaleTokenType.Identifier);
            return new IdentifierNode(token.Text);
        }

        internal DoubleLiteralNode DoubleLiteral()
        {
            AutoScaleToken token = _lexer.Consume(AutoScaleTokenType.DoubleLiteral);
            return new DoubleLiteralNode(double.Parse(token.Text));
        }

        internal StringLiteralNode StringLiteral()
        {
            AutoScaleToken token = _lexer.Consume(AutoScaleTokenType.StringLiteral);
            return new StringLiteralNode(token.Text);
        }
    }
}
