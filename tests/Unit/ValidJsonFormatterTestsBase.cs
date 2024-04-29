using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Unit;

public abstract class
    ValidJsonFormatterTestsBase<TFormatter, TFormatterOptions> : FormatterTestsBase<TFormatter,
    TFormatterOptions>
    where TFormatter : ConsoleFormatter
    where TFormatterOptions : JsonConsoleFormatterOptions, new()
{
    protected ValidJsonFormatterTestsBase(SpecBase<TFormatterOptions> spec, ITestOutputHelper testOutputHelper) : base(
        spec, testOutputHelper)
    {
    }

    [Fact]
    public void HelloWorld()
    {
        // Arrange
        var logger = LoggerBuilder.Build();

        // Act
        logger.LogInformation("Hello, world!");

        // Assert
        logger.Formatted.Should().Contain("Hello, world!");
        logger.Formatted.Should().BeValidJson();
    }

    [Theory]
    [InlineData("Hello, world!")]
    [InlineData("{Message}")]
    public void SimpleStringMessage_ValidJson(string message)
    {
        // Arrange
        var logger = LoggerBuilder.Build();

        // Act
        logger.LogInformation(message);

        // Assert
        logger.Formatted.Should().BeValidJson();
    }
}
