// Copyright (c) Laurence J. Golding. All rights reserved. Licensed under the Apache License, Version 2.0. See the LICENSE file in the project root for license information.
namespace Lakewood.AutoScale
{
    public enum AutoScaleTokenType
    {
        Unknown = 0,
        End,
        Comma,
        Identifier,
        Keyword,
        LineComment,
        DoubleLiteral,
        OperatorAddition,
        OperatorAssign,
        OperatorDivision,
        OperatorEquality,
        OperatorGreaterThan,
        OperatorGreaterThanOrEqual,
        OperatorLessThan,
        OperatorLessThanOrEqual,
        OperatorLogicalAnd,
        OperatorLogicalOr,
        OperatorMemberSelect,
        OperatorMultiplication,
        OperatorNot,
        OperatorNotEqual,
        OperatorSubtraction,
        OperatorTernaryColon,
        OperatorTernaryQuestion,
        ParenClose,
        ParenOpen,
        Semicolon,
        StringLiteral,
        WhiteSpace
    }
}
