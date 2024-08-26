namespace JsonConsoleFormatters.CategoryShortening;

/// <summary>
///     Interface for category shortening.
/// </summary>
internal interface ICategoryShortener
{
    /// <summary>
    ///     Shortens the given category.
    /// </summary>
    /// <param name="category">The category to shorten.</param>
    /// <returns>A read-only span of bytes representing the shortened category.</returns>
    public ReadOnlySpan<byte> Shorten(string category);
}
