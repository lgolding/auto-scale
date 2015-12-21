using Microsoft.VisualStudio.TextManager.Interop;

namespace Lakewood.AutoScaleFormulaLanguageService
{
    public class BraceMatch
    {
        private readonly TextSpan _start;
        private readonly TextSpan _end;
        private readonly int _priority;

        public BraceMatch(TextSpan start, TextSpan end, int priority = 0)
        {
            _start = start;
            _end = end;
            _priority = priority;
        }

        public TextSpan Start => _start;
        public TextSpan End => _end;
        public int Priority => _priority;
    }
}