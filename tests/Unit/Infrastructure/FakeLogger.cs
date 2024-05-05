using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Unit.Infrastructure;

public class FakeLogger : ILogger
{
    public FakeLogger(ConsoleFormatter formatter)
    {
        Formatter = formatter;
    }

    public Action<string?>? OnLogFormatted { get; set; }

    public ITestOutputHelper? TestOutputHelper { get; set; }

    public string Name { get; set; } = C.DefaultLoggerName;
    public ConsoleFormatter Formatter { get; }

    public IExternalScopeProvider? ScopeProvider { get; set; }

    public string? Formatted { get; private set; }

    public Action<StringWriter>? FormatterWriteAction { get; private set; }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var logEntry = new LogEntry<TState>(logLevel, Name, eventId, state, exception, formatter);

        FormatterWriteAction = stringWriter => Formatter.Write(in logEntry, ScopeProvider, stringWriter);

        using var stringWriter = new StringWriter(new StringBuilder(1024));
        FormatterWriteAction(stringWriter);
        Formatted = stringWriter.ToString();
        TestOutputHelper?.WriteLine(Formatted);
        OnLogFormatted?.Invoke(Formatted);
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => ScopeProvider?.Push(state);
}
