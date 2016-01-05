using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lakewood.AutoScale.Diagnostics;
using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale
{
    internal static class ParserError
    {
        public static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor("ASF0001", Severity.Error);

        private static readonly Dictionary<AutoScaleTokenType, string> s_tokenTypeToReadableStringDictionary = new Dictionary<AutoScaleTokenType, string>
        {
            [AutoScaleTokenType.Comma] = Resources.TokenTypeComma,
            [AutoScaleTokenType.DoubleLiteral] = Resources.TokenTypeDoubleLiteral,
            [AutoScaleTokenType.End] = Resources.TokenTypeEnd,
            [AutoScaleTokenType.Identifier] = Resources.TokenTypeIdentifier,
            [AutoScaleTokenType.Keyword] = Resources.TokenTypeKeyword,
            [AutoScaleTokenType.LineComment] = Resources.TokenTypeLineComment,
            [AutoScaleTokenType.OperatorAddition] = Resources.TokenTypeOperatorAddition,
            [AutoScaleTokenType.OperatorAssign] = Resources.TokenTypeOperatorAssign,
            [AutoScaleTokenType.OperatorDivision] = Resources.TokenTypeOperatorDivision,
            [AutoScaleTokenType.OperatorEquality] = Resources.TokenTypeOperatorEquality,
            [AutoScaleTokenType.OperatorGreaterThan] = Resources.TokenTypeOperatorGreaterThan,
            [AutoScaleTokenType.OperatorGreaterThanOrEqual] = Resources.TokenTypeOperatorGreaterThanOrEqual,
            [AutoScaleTokenType.OperatorLessThan] = Resources.TokenTypeOperatorLessThan,
            [AutoScaleTokenType.OperatorLessThanOrEqual] = Resources.TokenTypeOperatorLessThanOrEqual,
            [AutoScaleTokenType.OperatorLogicalAnd] = Resources.TokenTypeOperatorLogicalAnd,
            [AutoScaleTokenType.OperatorLogicalOr] = Resources.TokenTypeOperatorLogicalOr,
            [AutoScaleTokenType.OperatorMemberSelect] = Resources.TokenTypeOperatorMemberSelect,
            [AutoScaleTokenType.OperatorMultiplication] = Resources.TokenTypeOperatorMultiplication,
            [AutoScaleTokenType.OperatorNot] = Resources.TokenTypeOperatorNot,
            [AutoScaleTokenType.OperatorNotEqual] = Resources.TokenTypeOperatorNotEqual,
            [AutoScaleTokenType.OperatorSubtraction] = Resources.TokenTypeOperatorSubtraction,
            [AutoScaleTokenType.OperatorTernaryColon] = Resources.TokenTypeOperatorTernaryColon,
            [AutoScaleTokenType.OperatorTernaryQuestion] = Resources.TokenTypeOperatorTernaryQuestion,
            [AutoScaleTokenType.ParenClose] = Resources.TokenTypeParenClose,
            [AutoScaleTokenType.ParenOpen] = Resources.TokenTypeParenOpen,
            [AutoScaleTokenType.Semicolon] = Resources.TokenTypeSemicolon,
            [AutoScaleTokenType.StringLiteral] = Resources.TokenTypeStringLiteral,
            [AutoScaleTokenType.Unknown] = Resources.TokenTypeUnknown,
            [AutoScaleTokenType.WhiteSpace] = Resources.TokenTypeWhiteSpace
        };

        internal static string UnexpectedTokenMessage(AutoScaleToken actualToken, AutoScaleTokenType expectedTokenType)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.ErrorUnexpectedToken,
                s_tokenTypeToReadableStringDictionary[expectedTokenType],
                s_tokenTypeToReadableStringDictionary[actualToken.Type],
                actualToken.Text);
        }

        internal static string UnexpectedTokenMessage(AutoScaleToken actualToken, params AutoScaleTokenType[] expectedTokenTypes)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Resources.ErrorUnexpectedTokenWithChoices,
                string.Join(", ", expectedTokenTypes.Select(tt => s_tokenTypeToReadableStringDictionary[tt])),
                s_tokenTypeToReadableStringDictionary[actualToken.Type],
                actualToken.Text);
        }
    }
}
