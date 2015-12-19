using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Lakewood.AutoScaleFormulaLanguageService
{
    internal class AutoScaleFormulaScanner : IScanner
    {
        private readonly IVsTextLines _buffer;
        private string _source;

        public AutoScaleFormulaScanner(IVsTextLines buffer)
        {
            _buffer = buffer;
        }

        bool IScanner.ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            tokenInfo.Type = TokenType.Comment;
            tokenInfo.Color = TokenColor.Comment;
            return true;
        }

        void IScanner.SetSource(string source, int offset)
        {
            _source = source.Substring(offset);
        }
    }
}
