namespace Lakewood.AutoScale
{
    public class AutoScaleDeclaration
    {
        private readonly string _name;
        private readonly string _description;
        private readonly int _typeImageIndex;

        public AutoScaleDeclaration(string name, string description, int typeImageIndex)
        {
            _name = name;
            _description = description;
            _typeImageIndex = typeImageIndex;
        }

        public AutoScaleDeclaration(string name, string description, IconImageIndex iconImageIndex = default(IconImageIndex))
            : this(name, description, (int)iconImageIndex)
        {
        }

        public string Name => _name;
        public string Description => _description;
        public int TypeImageIndex => _typeImageIndex;
    }
}
