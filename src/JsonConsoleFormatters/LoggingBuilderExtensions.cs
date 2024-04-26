using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JsonConsoleFormatters;

public static class LoggingBuilderExtensions
{
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
