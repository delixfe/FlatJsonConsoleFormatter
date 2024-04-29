using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JsonConsoleFormatters;

/// <summary>
///     Provides extension methods for setting up the JeapJsonConsole formatter in am <see cref="ILoggingBuilder" />
/// </summary>
public static class LoggingBuilderExtensions
{
    /// <summary>
    ///     Adds the JeapJsonConsole formatter to the <see cref="ILoggingBuilder" />.
    /// </summary>
    /// <param name="builder">The <see cref="ILoggingBuilder" /> to add the formatter to.</param>
    /// <param name="configure">The <see cref="JeapJsonConsoleFormatterOptions" /> configuration delegate.</param>
    /// <returns>The value of <paramref name="builder" />.</returns>
    public static ILoggingBuilder AddJeapJsonConsole(this ILoggingBuilder builder,
        Action<JeapJsonConsoleFormatterOptions>? configure = null)
    {
        builder.AddConsole(options => options.FormatterName = JeapJsonConsoleFormatter.FormatterName);
        builder.AddConsoleFormatter<JeapJsonConsoleFormatter, JeapJsonConsoleFormatterOptions>();
        if (configure != null)
            builder.Services.Configure(configure);
        return builder;
    }
}
