﻿// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lakewood.AutoScale
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

        internal static readonly string[] NodeDeallocationOptionKeywords = new[]
        {
            "requeue",
            "retaindata",
            "taskcompletion",
            "terminate"
        };

        private string _source;
        private int _index;

        public Lexer(string source)
        {
            _source = source;
        }

        public bool More()
        {
            return _index < _source.Length;
        }

        public AutoScaleToken GetNextToken()
        {
            return ReadNextToken();
        }

        public AutoScaleToken Peek()
        {
            var prevIndex = _index;
            AutoScaleToken peekedToken = ReadNextToken();
            _index = prevIndex;

            return peekedToken;
        }

        public void Skip()
        {
            ReadNextToken();
        }

        public void SkipWhite()
        {
            AutoScaleTokenType nextTokenType;
            while ((nextTokenType = Peek().Type) == AutoScaleTokenType.WhiteSpace || nextTokenType == AutoScaleTokenType.LineComment)
            {
                Skip();
            }
        }

        public AutoScaleToken Consume(AutoScaleTokenType tokenType)
        {
            SkipWhite();

            AutoScaleToken nextToken = Peek();
            if (nextToken.Type == tokenType)
            {
                Skip();
                return nextToken;
            }
            else
            {
                throw new ParserException(tokenType, nextToken);
            }
        }

        private AutoScaleToken ReadNextToken()
        {
            if (_index >= _source.Length)
            {
                return new AutoScaleToken(AutoScaleTokenType.End, _index, _index, string.Empty);
            }

            int startIndex = _index;
            AutoScaleTokenType type = AutoScaleTokenType.Unknown;

            char ch = _source[_index];

            if (char.IsWhiteSpace(ch))
            {
                type = AutoScaleTokenType.WhiteSpace;

                while (NextCharSatisfies(char.IsWhiteSpace))
                {
                    ++_index;
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
                        ++_index;
                    }

                    // Consume the trailing line break, if there was one.
                    if (_index < _source.Length - 1)
                    {
                        ++_index;
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
                    ++_index;
                }

                string identifier = GetTokenText(startIndex);
                type = IsNodeDeallocationOptionKeyword(identifier) ? AutoScaleTokenType.Keyword : AutoScaleTokenType.Identifier;
            }
            else if (char.IsDigit(ch))
            {
                type = AutoScaleTokenType.DoubleLiteral;
                ParseNumber();
            }
            else if (ch == '"')
            {
                type = ParseString()
                    ? AutoScaleTokenType.StringLiteral
                    : AutoScaleTokenType.Unknown;
            }
            else if (s_singleCharacterTokenDictionary.TryGetValue(ch, out type))
            {
                // Nothing more to do.
            }
            else if (s_logicalOperatorDictionary.TryGetValue(ch, out type))
            {
                if (NextCharIs(ch))
                {
                    ++_index;
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
                    ++_index;
                }
            }

            int endIndex = _index++;
            string text = _source.Substring(startIndex, _index - startIndex);

            return new AutoScaleToken(type, startIndex, endIndex, text);
        }

        private void ParseNumber()
        {
            while (NextCharSatisfies(char.IsDigit))
            {
                ++_index;
            }

            if (NextCharIs('.'))
            {
                ++_index;
                while (NextCharSatisfies(char.IsDigit))
                {
                    ++_index;
                }
            }
        }

        private bool ParseString()
        {
            bool validString = false;

            while (_index < _source.Length && !NextCharIs('"'))
            {
                ++_index;
            }

            if (NextCharIs('"'))
            {
                ++_index;
                validString = true;
            }
            else
            {
                --_index; // We ran off the end; back up.
            }

            return validString;
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

        private static bool IsNotLineBreak(char ch)
        {
            return ch != '\n' && ch != '\r';
        }

        internal static bool IsNodeDeallocationOptionKeyword(string s)
        {
            return NodeDeallocationOptionKeywords.Contains(s);
        }
    }
}
