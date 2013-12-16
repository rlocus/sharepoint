
using System.Globalization;

namespace SPCore.Formula.Base.Interfaces
{
    /// <summary>
    /// Marker interface. If you want to create you own element type please inherit from Element and use this IElement for parameters only.
    /// </summary>
    public interface IElementType
    {
        bool UseEnvironmentCulture { get; set; }
        CultureInfo Culture { get; set; }
    }
}
