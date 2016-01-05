using System.Globalization;

namespace Pollock
{
    public interface ITypeConverter
    {
        object FromString(CultureInfo culture, string value);
        string ToString(CultureInfo culture, object value);
    }
}
