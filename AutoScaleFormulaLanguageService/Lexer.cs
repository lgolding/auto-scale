using System;
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScaleFormulaLanguageService
{
    internal class Lexer
    {
        private static readonly Dictionary<char, AutoScaleTokenType> s_singleCharacterTokenDictionary = new Dictionary<char, AutoScaleTokenType>
        {
            [','] = AutoScaleTokenType.Comma,
            ['+'] = AutoScaleTokenType.OperatorAddition,
            ['-'] = AutoScaleTokenType.OperatorSubtraction,
            ['.'] = AutoScaleTokenType.OperatorMemberSelect,
            ['*'] = AutoScaleTokenType.OperatorMultiplication,
            [':'] = AutoScaleTokenType.OperatorTernaryColon,
            ['?'] = AutoScaleTokenType.OperatorTernaryQuestion,
            [')'] = AutoScaleTokenType.ParenClose,
            ['('] = AutoScaleTokenType.ParenOpen,
            [';'] = AutoScaleTokenType.Semicolon
        };

        private class AutoScaleTokenMapping
        {
            public AutoScaleTokenMapping(AutoScaleTokenType typeWithoutEqualsSign, AutoScaleTokenType typeWithEqualsSign)
            {
                TypeWithoutEqualsSign = typeWithoutEqualsSign;
                TypeWithEqualsSign = typeWithEqualsSign;
            }

            public AutoScaleTokenType TypeWithoutEqualsSign;
            public AutoScaleTokenType TypeWithEqualsSign;
        }

        private static readonly Dictionary<char, AutoScaleTokenMapping> s_operatorsWithOptionalEqualsDictionary = new Dictionary<char, AutoScaleTokenMapping>
        {
            ['>'] = new AutoScaleTokenMapping(AutoScaleTokenType.OperatorGreaterThan, AutoScaleTokenType.OperatorGreaterThanOrEqual),
            ['<'] = new AutoScaleTokenMapping(AutoScaleTokenType.OperatorLessThan, AutoScaleTokenType.OperatorLessThanOrEqual),
            ['='] = new AutoScaleTokenMapping(AutoScaleTokenType.OperatorAssign, AutoScaleTokenType.OperatorEquality),
            ['!'] = new AutoScaleTokenMapping(AutoScaleTokenType.OperatorNot, AutoScaleTokenType.OperatorNotEqual)
        };
            
        private static readonly Dictionary<char, AutoScaleTokenType> s_logicalOperatorDictionary = new Dictionary<char, AutoScaleTokenType>
        {
            ['&'] = AutoScaleTokenType.OperatorLogicalAnd,
            ['|'] = AutoScaleTokenType.OperatorLogicalOr
        };

        private static readonly string[] s_keywords = new[]
        {
            "requeue",
            "retaindata",
            "taskcompletion",
            "terminate"
        };

        private string _source;
        private int _line;
        private int _col;
        private int _index;
        private bool _lineBreakSeen;

        public Lexer(string source)
        {
            _source = source;
            _line = _col = _index = 0;
        }

        public AutoScaleToken GetNextToken()
        {
            if (_index >= _source.Length)
            {
                return null;
            }

            int line = _line;
            int col = _col;
            int startIndex = _index;
            AutoScaleTokenType type = AutoScaleTokenType.Unknown;

            char ch = _source[_index];

            if (char.IsWhiteSpace(ch))
            {
                type = AutoScaleTokenType.WhiteSpace;

                while (NextCharSatisfies(char.IsWhiteSpace))
                {
                    Advance();
                }
            }
            else if (ch == '/')
            {
                // Disambiguate division operator from comment.
                if (NextCharIs('/'))
                {
                    type = AutoScaleTokenType.LineComment;

                    // Comment extends to end of line.
                    while (NextCharSatisfies(IsNotLineBreak))
                    {
                        Advance();
                    }
                }
                else
                {
                    type = AutoScaleTokenType.OperatorDivision;
                }
            }
            else if (IsLeadingIdentifierCharacter(ch))
            {
                type = AutoScaleTokenType.Identifier;

                while (NextCharSatisfies(IsIdentifierCharacter))
                {
                    Advance();
                }

                string identifier = GetTokenText(startIndex);
                type = s_keywords.Contains(identifier) ? AutoScaleTokenType.Keyword : AutoScaleTokenType.Identifier;
            }
            else if (char.IsDigit(ch))
            {
                type = AutoScaleTokenType.Literal;
                ParseNumber();
            }
            else if (s_singleCharacterTokenDictionary.TryGetValue(ch, out type))
            {
                // Nothing more to do.
            }
            else if (s_logicalOperatorDictionary.TryGetValue(ch, out type))
            {
                if (NextCharIs(ch))
                {
                    Advance();
                }
                else
                {
                    type = AutoScaleTokenType.Unknown;
                }
            }
            else if (s_operatorsWithOptionalEqualsDictionary.Keys.Contains(ch))
            {
                var value = s_operatorsWithOptionalEqualsDictionary[ch];
                type = value.TypeWithoutEqualsSign;

                if (NextCharIs('='))
                {
                    type = value.TypeWithEqualsSign;
                    Advance();
                }
            }

            int endIndex = _index;
            string text = _source.Substring(startIndex, _index - startIndex + 1);

            Advance();

            return new AutoScaleToken(type, line, col, startIndex, endIndex, text);
        }

        private void ParseNumber()
        {
            while (NextCharSatisfies(char.IsDigit))
            {
                Advance();
            }

            if (NextCharIs('.'))
            {
                Advance();
                while (NextCharSatisfies(char.IsDigit))
                {
                    Advance();
                }
            }
        }

        private string GetTokenText(int startIndex)
        {
            return _source.Substring(startIndex, _index - startIndex + 1);
        }

        private bool NextCharSatisfies(Func<char, bool> predicate)
        {
            return _index < _source.Length - 1 && predicate(_source[_index + 1]);
        }

        private bool NextCharIs(char ch)
        {
            return NextCharSatisfies(c => c == ch);
        }

        private static bool IsLeadingIdentifierCharacter(char ch)
        {
            return char.IsLetter(ch) || ch == '_' || ch == '$';
        }

        private static bool IsIdentifierCharacter(char ch)
        {
            return char.IsLetterOrDigit(ch) || ch == '_';
        }

        private static bool IsLineBreak(char ch)
        {
            return ch == '\n' || ch == '\r';
        }

        private static bool IsNotLineBreak(char ch)
        {
            return !IsLineBreak(ch);
        }

        private void Advance()
        {
            if (++_index < _source.Length)
            {
                if (_lineBreakSeen)
                {
                    ++_line;
                    _col = 0;
                    _lineBreakSeen = false;
                }
                else
                {
                    ++_col;
                }

                char ch = _source[_index];
                if (IsLineBreak(ch))
                {
                    _lineBreakSeen = true;
                }
            }
        }
    }
}
