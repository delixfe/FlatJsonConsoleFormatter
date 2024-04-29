# JsonConsoleFormatters

This project contains a custom log message formatters for the `Microsoft.Extensions.Logging.Console` package.

```bash
    dotnet add package JeapJsonConsoleFormatter
```

This project builds on https://github.com/nick-durcholz-vectorsolutions/FlatJsonConsoleFormatter.
It includes source code from `FlatJsonConsoleFormatter` and Microsoft's `Microsoft.Extensions.Logging.Console`.

## JeapJsonConsoleFormatter

`JeapJsonConsoleFormatter` is an opinionated log message formatter based on SprintBoot's default log message format.

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



