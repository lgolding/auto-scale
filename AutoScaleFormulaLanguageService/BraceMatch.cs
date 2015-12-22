namespace Lakewood.AutoScaleFormulaLanguageService
{
    public class BraceMatch
    {
        private readonly int _leftIndex;
        private readonly int _rightIndex;

        public BraceMatch(int leftIndex, int rightIndex)
        {
            _leftIndex = leftIndex;
            _rightIndex = rightIndex;
        }

        public int Left => _leftIndex;
        public int Right => _rightIndex;
    }
}