using JsonConsoleFormatters.CategoryShortening;

namespace Unit.JeapJsonFormatter.CategoryShortening;

public class CachingCategoryShortenerTests
{
    [Fact]
    public void Shortens_ReturnsCachedInstances()
    {
        const int threadCount = 10;
        const int iterationsPerThread = 1000;
        const int categoryCount = 100;

        // Arrange
        var innerShortener = new RenameCategoryShortener(
            new Dictionary<string, string> { ["LongName."] = "L." });
        var subject = new CachingCategoryShortener(innerShortener);
        var inputs = Enumerable.Range(0, categoryCount).Select(i => $"LongName.{i:000}").ToArray();
        var expectedByteArrays = inputs.Select(input => innerShortener.Shorten(input)).ToArray();
        var threads = new Thread[threadCount];
        Exception? exception = null;


        // Act and Assert
        var action = () =>
        {
            try
            {
                for (var i = 0; i < iterationsPerThread; i++)
                {
                    var categoryIndex = i % categoryCount;

                    var actual = subject.Shorten(inputs[categoryIndex]);
                    var expectedBytes = expectedByteArrays[categoryIndex];

                    Assert.Same(expectedBytes, actual);
                }
            }
            catch (Exception e)
            {
                exception = e;
            }
        };

        for (var i = 0; i < threadCount; i++)
        {
            threads[i] = new Thread(new ThreadStart(action));
            threads[i].Start();
        }

        Array.ForEach(threads, t => t.Join());

        if (exception != null)
        {
            throw exception;
        }
    }
}
