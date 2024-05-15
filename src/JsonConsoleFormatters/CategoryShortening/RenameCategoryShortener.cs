using System.Text;

namespace JsonConsoleFormatters.CategoryShortening;

/// <summary>
///     Class for renaming categories.
///     Implements the <see cref="ICategoryShortener" /> interface.
/// </summary>
internal class RenameCategoryShortener : ICategoryShortener
{
    private readonly Dictionary<string, string> _config;
    private readonly Encoding _encoding = new UTF8Encoding(false);

    /// <summary>
    ///     Initializes a new instance of the <see cref="RenameCategoryShortener" /> class.
    /// </summary>
    /// <param name="mappings">The mappings to use for renaming categories.</param>
    public RenameCategoryShortener(IEnumerable<KeyValuePair<string, string>> mappings)
    {
        _config = new Dictionary<string, string>(mappings);
    }


    /// <summary>
    ///     Shortens the given category by renaming it according to the mappings provided in the constructor.
    /// </summary>
    /// <param name="category">The category to shorten.</param>
    /// <returns>A read-only span of bytes representing the shortened category.</returns>
    public byte[] Shorten(string category)
    {
        if (category.Length == 0)
        {
            return Array.Empty<byte>();
        }


        foreach (var (source, target) in _config)
        {
            if (category.StartsWith(source))
            {
                var input = category.AsSpan()[source.Length..];
                var replacement = target.AsSpan();
                // ReSharper disable once StackAllocInsideLoop
                Span<char> shortened = stackalloc char[replacement.Length + input.Length];
                replacement.CopyTo(shortened);
                input.CopyTo(shortened[replacement.Length..]);

                var needed = _encoding.GetByteCount(shortened);
                var buffer = new byte[needed];
                if (_encoding.TryGetBytes(shortened, buffer, out var bytesWritten))
                {
                    if (bytesWritten < needed)
                    {
                        return buffer[..bytesWritten];
                    }

                    return buffer;
                }

                return _encoding.GetBytes(shortened.ToArray());
            }
        }

        return _encoding.GetBytes(category);
    }
}
