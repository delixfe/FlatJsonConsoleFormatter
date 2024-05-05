using System.Text;
using JsonConsoleFormatters.CategoryShortening;

namespace Unit.JeapJsonFormatter.CategoryShortening;

public class RenameCategoryShortenerTests
{
    [Fact]
    public void Shorten_Empty_ReturnsEmpty()
    {
        // Arrange
        var mappings = new Dictionary<string, string> { ["Long."] = "L." };
        var subject = new RenameCategoryShortener(mappings);

        // Act
        var actual = subject.Shorten(string.Empty);

        // Assert
        actual.Length.Should().Be(0);
    }

    [Theory]
    [InlineData("Source.", "", "Source.A", "A")]
    [InlineData("Source.", "S.", "Source.A", "S.A")]
    [InlineData("Source.", "S.", "Abc.A", "Abc.A")]
    [InlineData("Source.", "S.", "Abc.Source.A", "Abc.Source.A")]
    [InlineData("Source.", "S.", "Source.Source.A", "S.Source.A")]
    public void Shorten_SourceTargetInputExpected(string source, string target, string input, string expected)
    {
        // Arrange
        var mappings = new Dictionary<string, string> { [source] = target };
        var subject = new RenameCategoryShortener(mappings);

        // Act
        var actual = subject.Shorten(input);

        // Assert
        Assert.Equal(expected, Encoding.UTF8.GetString(actual));
        Assert.Equal(Encoding.UTF8.GetBytes(expected), actual.ToArray());
    }
}
