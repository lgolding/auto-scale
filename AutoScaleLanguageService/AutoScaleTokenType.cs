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
