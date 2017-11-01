using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPSLocal.Classes
{
    public struct ChessAnnotation
    {
        public bool IsNumericGlyph { get; }
        public int? NAG { get; }
        public string Annotation { get; }

        public ChessAnnotation(int? nag)
        {
            IsNumericGlyph = true;
            NAG = nag;
            Annotation = null;
        }

        public ChessAnnotation(string annotation)
        {
            IsNumericGlyph = false;
            NAG = null;
            Annotation = annotation;
        }

        public override string ToString()
        {
            if (IsNumericGlyph)
            {
                return $"${NAG}";
            }
            return $"{{{Annotation}}}";
        }
    }
}
