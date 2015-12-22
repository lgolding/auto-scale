namespace Lakewood.AutoScaleFormulaLanguageService
{
    public enum AutoScaleTokenType
    {
        Unknown = 0,
        Comma,
        Identifier,
        Keyword,
        LineComment,
        Literal,
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
        WhiteSpace
    }
}
