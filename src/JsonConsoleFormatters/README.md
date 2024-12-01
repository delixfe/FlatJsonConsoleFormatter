# JsonConsoleFormatters

This project contains a custom log message formatters for the `Microsoft.Extensions.Logging.Console` package.

```bash
    dotnet add package JeapJsonConsoleFormatter
```

This project builds on https://github.com/nick-durcholz-vectorsolutions/FlatJsonConsoleFormatter.
It includes source code from `FlatJsonConsoleFormatter` and Microsoft's `Microsoft.Extensions.Logging.Console`.

## JeapJsonConsoleFormatter

`JeapJsonConsoleFormatter` is an opinionated log message formatter based on SprintBoot's default log message format.

### Fields

* `@timestamp` - The time the log event was written by the formatter in ISO 8601
  ([round-trip specifier "O"](https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings#Roundtrip)).
* `level` - The log level formatted as
  [Otel SeverityText](https://opentelemetry.io/docs/specs/otel/logs/data-model/#field-severitytext).
* `severity` -
  [Otel SeverityNumber](https://opentelemetry.io/docs/specs/otel/logs/data-model/#field-severitynumber).
* `logger` - The logger category.
* `message` - The formatted log message.
* `eventId` - The event id, if it is greater than 0.
* `eventName` - The event name, if it is present.

__Optional fields__

* `sequence` - Monotonic increasing counter (if enabled, default true).
* `thread_name` - The thread name (if enabled, default false).

Additional features:

* Output of the thread name (if enabled)
* TBD

### Usage

Configure logging to use the JeapJsonConsoleFormatter.

```csharp
    var services = new ServiceCollection();
    services.AddLogging(builder =>
    {
        builder.AddConfiguration(configuration.GetSection("Logging"));
        builder.AddJeapJsonConsole(o =>
        {
            o.IncludeThreadName = false;
        });
    });
```



