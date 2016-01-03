using System.Linq;

namespace Lakewood.AutoScale.Diagnostics
{
    internal static class Extensions
    {
        internal static bool IsNodeDeallocationOptionKeyword(this string s)
        {
            return Lexer.NodeDeallocationOptionKeywords.Contains(s);
        }
    }
}
