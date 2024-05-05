using System.Text.Json;
using JsonConsoleFormatters;

namespace Unit.JeapJsonFormatter;

public class
    JeapJsonFormatterTests_GetCamelCasedUniqueKey
{
    [Theory]
    [InlineData("KeyKey", "keyKey")]
    [InlineData("keyKey", "keyKey")]
    public void Keys_AreCamelCased(string key, string expected)
    {
        // Arrange
        var writtenNames = new HashSet<string>();

        // Act
        var actual = JeapJsonConsoleFormatter.GetCamelCasedUniqueKey(key, writtenNames);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("Key")]
    [InlineData("key")]
    [InlineData("Category")]
    [InlineData("LogLevel")]
    public void UnReservedKeys_AreSuffixedAfterTheFirst(string key)
    {
        // Arrange
        var camelCasedKey = JsonNamingPolicy.CamelCase.ConvertName(key);
        var writtenNames = new HashSet<string>();
        var actual = new List<string>();

        // Act
        for (var i = 0; i < 101; i++)
        {
            var uniqueKey = JeapJsonConsoleFormatter.GetCamelCasedUniqueKey(key, writtenNames);
            actual.Add(uniqueKey);
        }

        // Assert
        actual[0].Should().Be($"{camelCasedKey}");
        actual[1].Should().Be($"{camelCasedKey}_1");
        actual[2].Should().Be($"{camelCasedKey}_2");
        actual[10].Should().Be($"{camelCasedKey}_10");
        actual[100].Should().Be($"{camelCasedKey}_100");
    }

    [Theory]
    [InlineData("@timestamp")]
    [InlineData("level")]
    [InlineData("message")]
    [InlineData("eventId")]
    [InlineData("exception")]
    [InlineData("severity")]
    [InlineData("eventName")]
    // mapping to reserved keys
    [InlineData("Message")]
    [InlineData("EventId")]
    [InlineData("Exception")]
    [InlineData("Severity")]
    [InlineData("EventName")]
    public void ReservedKeys_AreSuffixed(string reservedKey)
    {
        // Arrange
        var camelCasedKey = JsonNamingPolicy.CamelCase.ConvertName(reservedKey);
        var writtenNames = new HashSet<string>();
        var actual = new List<string>();

        // Act
        for (var i = 0; i < 101; i++)
        {
            var uniqueKey = JeapJsonConsoleFormatter.GetCamelCasedUniqueKey(reservedKey, writtenNames);
            actual.Add(uniqueKey);
        }

        // Assert
        actual[0].Should().Be($"{camelCasedKey}_1");
        actual[1].Should().Be($"{camelCasedKey}_2");
        actual[2].Should().Be($"{camelCasedKey}_3");
        actual[11].Should().Be($"{camelCasedKey}_12");
        actual[100].Should().Be($"{camelCasedKey}_101");
    }
}
