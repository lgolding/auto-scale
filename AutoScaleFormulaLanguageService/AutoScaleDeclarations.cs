using System.Collections.Generic;
using Microsoft.VisualStudio.Package;

namespace Lakewood.AutoScale
{
    internal class AutoScaleDeclarations : Declarations
    {
        private List<AutoScaleDeclaration> _declarations = new List<AutoScaleDeclaration>();

        public void AddDeclaration(AutoScaleDeclaration declaration)
        {
            _declarations.Add(declaration);
        }

        #region Declarations Methods

        public override int GetCount()
        {
            return _declarations.Count;
        }

        public override string GetDescription(int index)
        {
            return IsValidIndex(index)
                ? _declarations[index].Description
                : string.Empty;
        }

        public override string GetDisplayText(int index)
        {
            return GetName(index);
        }

        public override int GetGlyph(int index)
        {
            return IsValidIndex(index)
                ? _declarations[index].TypeImageIndex
                :-1;
        }

        public override string GetName(int index)
        {
            return IsValidIndex(index)
                ? _declarations[index].Name
                : string.Empty;
        }

        #endregion Declarations Methods

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < _declarations.Count;
        }
    }
}
