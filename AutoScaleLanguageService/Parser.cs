using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using Lakewood.AutoScale.Diagnostics;
using Lakewood.AutoScale.Syntax;

namespace Lakewood.AutoScale
{
    internal class Parser
    {
        private readonly Lexer _lexer;
        private List<Diagnostic> _diagnostics = new List<Diagnostic>();

        // MEF will fill the list of rules, so suppress the compiler warning that _importedRules
        // "is never assigned to, and will always have its default value of null".
        #pragma warning disable 0649
        [ImportMany]
        private IEnumerable<Lazy<IDiagnosticRule>> _importedRules;
        #pragma warning restore 0649

        private IEnumerable<IDiagnosticRule> DiagnosticRules => _importedRules.Select(ir => ir.Value);

        internal Parser(string input)
        {
            _lexer = new Lexer(input);
            ImportRules();
        }

        public IReadOnlyCollection<Diagnostic> Diagnostics => Array.AsReadOnly(_diagnostics.ToArray());

        internal FormulaNode Parse()
        {
            var assignments = new List<AssignmentNode>();

            _lexer.SkipWhite();
            while (_lexer.More())
            {
                try
                {
                    assignments.Add(Assignment());

                    _lexer.SkipWhite();
                    if (_lexer.More())
                    {
                        _lexer.Consume(AutoScaleTokenType.Semicolon);
                        _lexer.SkipWhite();
                    }
                }
                catch (ParserException ex)
                {
                    _diagnostics.Add(new Diagnostic(ParserError.Descriptor, ex.Message, ex.StartIndex, ex.EndIndex));
                    SkipToEndOfStatement();
                    _lexer.SkipWhite();
                }
            }

            var formula = new FormulaNode(assignments.ToArray());

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            formula.Accept(new DiagnosticVisitor(DiagnosticRules));
            watch.Stop();

            _diagnostics.AddRange(DiagnosticRules.SelectMany(r => r.Diagnostics));

            return formula;
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

                return new TernaryOperationNode(condition, trueValue, falseValue);
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
            var left = AdditiveExpression();

            while (_lexer.More())
            {
                _lexer.SkipWhite();

                var nextTokenType = _lexer.Peek().Type;
                BinaryOperator comparisonOperator = ComparisonOperatorFromTokenType(nextTokenType);
                if (comparisonOperator != BinaryOperator.Unknown)
                {
                    _lexer.Skip();
                    var right = AdditiveExpression();

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
            return BinaryOperatorFromTokenType(tokenType, s_comparisonOperatorDictionary);
        }

        private SyntaxNode AdditiveExpression()
        {
            var left = MultiplicativeExpression();

            while (_lexer.More())
            {
                _lexer.SkipWhite();

                var nextTokenType = _lexer.Peek().Type;
                BinaryOperator additiveOperator = AdditiveOperatorFromTokenType(nextTokenType);
                if (additiveOperator != BinaryOperator.Unknown)
                {
                    _lexer.Skip();
                    var right = MultiplicativeExpression();

                    left = new BinaryOperationNode(additiveOperator, left, right);
                }
                else
                {
                    break;
                }
            }

            return left;
        }

        private static readonly IDictionary<AutoScaleTokenType, BinaryOperator> s_additiveOperatorDictionary = new Dictionary<AutoScaleTokenType, BinaryOperator>
        {
            [AutoScaleTokenType.OperatorAddition] = BinaryOperator.Addition,
            [AutoScaleTokenType.OperatorSubtraction] = BinaryOperator.Subtraction
        };

        private BinaryOperator AdditiveOperatorFromTokenType(AutoScaleTokenType tokenType)
        {
            return BinaryOperatorFromTokenType(tokenType, s_additiveOperatorDictionary);
        }

        private SyntaxNode MultiplicativeExpression()
        {
            var left = UnaryExpression();

            while (_lexer.More())
            {
                _lexer.SkipWhite();

                var nextTokenType = _lexer.Peek().Type;
                BinaryOperator multiplicativeOperator = MultiplicativeOperatorFromTokenType(nextTokenType);
                if (multiplicativeOperator != BinaryOperator.Unknown)
                {
                    _lexer.Skip();
                    var right = UnaryExpression();

                    left = new BinaryOperationNode(multiplicativeOperator, left, right);
                }
                else
                {
                    break;
                }
            }

            return left;
        }

        private static readonly IDictionary<AutoScaleTokenType, BinaryOperator> s_multiplicativeOperatorDictionary = new Dictionary<AutoScaleTokenType, BinaryOperator>
        {
            [AutoScaleTokenType.OperatorMultiplication] = BinaryOperator.Multiplication,
            [AutoScaleTokenType.OperatorDivision] = BinaryOperator.Division
        };

        private BinaryOperator MultiplicativeOperatorFromTokenType(AutoScaleTokenType tokenType)
        {
            return BinaryOperatorFromTokenType(tokenType, s_multiplicativeOperatorDictionary);
        }

        private BinaryOperator BinaryOperatorFromTokenType(
            AutoScaleTokenType tokenType,
            IDictionary<AutoScaleTokenType, BinaryOperator> dictionary)
        {
            BinaryOperator @operator;
            if (!dictionary.TryGetValue(tokenType, out @operator))
            {
                @operator = BinaryOperator.Unknown;
            }

            return @operator;
        }

        private SyntaxNode UnaryExpression()
        {
            _lexer.SkipWhite();

            AutoScaleToken unaryOperatorToken = _lexer.Peek();
            UnaryOperator unaryOperator = UnaryOperatorFromTokenType(unaryOperatorToken.Type);
            if (unaryOperator != UnaryOperator.Unknown)
            {
                int startIndex = unaryOperatorToken.StartIndex;

                _lexer.Skip();
                var operand = UnaryExpression(); // They can be nested, e.g., "-!-a".
                int endIndex = operand.EndIndex;

                return new UnaryOperationNode(unaryOperatorToken, unaryOperator, operand);
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

                case AutoScaleTokenType.Keyword:
                    return Keyword();

                case AutoScaleTokenType.Identifier:
                    SyntaxNode result = Identifier();

                    _lexer.SkipWhite();
                    var nextTokenType = _lexer.Peek().Type;
                    if (nextTokenType == AutoScaleTokenType.ParenOpen)
                    {
                        result = FunctionCall(result as IdentifierNode);
                    }
                    else if (nextTokenType == AutoScaleTokenType.OperatorMemberSelect)
                    {
                        _lexer.Skip();
                        result = MethodInvocation(result as IdentifierNode);
                    }

                    return result;

                case AutoScaleTokenType.ParenOpen:
                    return ParenthesizedExpression();

                default:
                    throw new ParserException(
                        nextToken.StartIndex,
                        nextToken.EndIndex,
                        ParserError.UnexpectedTokenMessage(
                            nextToken,
                            AutoScaleTokenType.DoubleLiteral,
                            AutoScaleTokenType.StringLiteral,
                            AutoScaleTokenType.Identifier));
            }
        }

        internal SyntaxNode ParenthesizedExpression()
        {
            var openParen = _lexer.Consume(AutoScaleTokenType.ParenOpen);
            var innerExpression = Expression();
            var closeParen = _lexer.Consume(AutoScaleTokenType.ParenClose);

            return new ParenthesizedExpressionNode(openParen, innerExpression, closeParen);
        }

        private FunctionCallNode FunctionCall(IdentifierNode identfier)
        {
            var arguments = new List<SyntaxNode>();

            _lexer.Consume(AutoScaleTokenType.ParenOpen);
            AutoScaleToken closeParen = null;

            _lexer.SkipWhite();

            var nextToken = _lexer.Peek();
            if (nextToken.Type == AutoScaleTokenType.ParenClose)
            {
                closeParen = nextToken;
                _lexer.Skip();
            }
            else
            {
                while (_lexer.More())
                {
                    var arg = Expression();
                    arguments.Add(arg);

                    _lexer.SkipWhite();
                    nextToken = _lexer.Peek();
                    if (nextToken.Type == AutoScaleTokenType.Comma)
                    {
                        _lexer.Skip();
                        _lexer.SkipWhite();
                    }
                    else if (nextToken.Type == AutoScaleTokenType.ParenClose)
                    {
                        closeParen = nextToken;
                        _lexer.Skip();
                        break;
                    }
                    else
                    {
                        throw new ParserException(
                            nextToken.StartIndex,
                            nextToken.EndIndex,
                            ParserError.UnexpectedTokenMessage(
                                nextToken,
                                AutoScaleTokenType.Comma,
                                AutoScaleTokenType.ParenClose));
                    }
                }
            }

            if (closeParen == null)
            {
                throw new ParserException(nextToken.StartIndex, nextToken.EndIndex, Resources.ErrorUnexpectedEndOfFile);
            }

            return new FunctionCallNode(identfier, arguments, closeParen);
        }

        private MethodInvocationNode MethodInvocation(IdentifierNode @object)
        {
            var arguments = new List<SyntaxNode>();

            var method = Identifier();

            _lexer.Consume(AutoScaleTokenType.ParenOpen);
            AutoScaleToken closeParen = null;

            _lexer.SkipWhite();

            var nextToken = _lexer.Peek();
            if (nextToken.Type == AutoScaleTokenType.ParenClose)
            {
                closeParen = nextToken;
                _lexer.Skip();
            }
            else
            {
                while (_lexer.More())
                {
                    var arg = Expression();
                    arguments.Add(arg);

                    _lexer.SkipWhite();
                    nextToken = _lexer.Peek();
                    if (nextToken.Type == AutoScaleTokenType.Comma)
                    {
                        _lexer.Skip();
                        _lexer.SkipWhite();
                    }
                    else if (nextToken.Type == AutoScaleTokenType.ParenClose)
                    {
                        closeParen = nextToken;
                        _lexer.Skip();
                        break;
                    }
                    else
                    {
                        throw new ParserException(
                            nextToken.StartIndex,
                            nextToken.EndIndex,
                            ParserError.UnexpectedTokenMessage(
                                nextToken,
                                AutoScaleTokenType.Comma,
                                AutoScaleTokenType.ParenClose));
                    }
                }
            }

            if (closeParen == null)
            {
                throw new ParserException(nextToken.StartIndex, nextToken.EndIndex, Resources.ErrorUnexpectedEndOfFile);
            }

            return new MethodInvocationNode(@object, method, arguments, closeParen);
        }

        internal IdentifierNode Identifier()
        {
            AutoScaleToken token = _lexer.Consume(AutoScaleTokenType.Identifier);
            return new IdentifierNode(token);
        }

        internal DoubleLiteralNode DoubleLiteral()
        {
            AutoScaleToken token = _lexer.Consume(AutoScaleTokenType.DoubleLiteral);
            return new DoubleLiteralNode(token);
        }

        internal StringLiteralNode StringLiteral()
        {
            AutoScaleToken token = _lexer.Consume(AutoScaleTokenType.StringLiteral);
            return new StringLiteralNode(token);
        }

        internal KeywordNode Keyword()
        {
            AutoScaleToken token = _lexer.Consume(AutoScaleTokenType.Keyword);
            return new KeywordNode(token);
        }

        private void SkipToEndOfStatement()
        {
            while (_lexer.More() && _lexer.Peek().Type != AutoScaleTokenType.Semicolon)
            {
                _lexer.Skip();
            }

            // Skip the semicolon, if we haven't run off the end.
            if (_lexer.More())
            {
                _lexer.Skip();
            }
        }

        private void ImportRules()
        {
            var catalog = new AssemblyCatalog(GetType().Assembly);
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
        }
    }
}
