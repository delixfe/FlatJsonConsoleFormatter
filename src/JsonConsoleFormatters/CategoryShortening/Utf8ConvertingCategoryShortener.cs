using System.Text;

namespace JsonConsoleFormatters.CategoryShortening;

internal class Utf8ConvertingCategoryShortener : ICategoryShortener
{
    private readonly Encoding _encoding = new UTF8Encoding(false);

    public ReadOnlySpan<byte> Shorten(string category) => _encoding.GetBytes(category);
}
