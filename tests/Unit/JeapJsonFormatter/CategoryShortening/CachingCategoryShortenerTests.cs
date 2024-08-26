using System.Collections.Concurrent;
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
        var inputs = Enumerable.Range(0, categoryCount).Select(i => $"LongName.{i:000}").ToList();
        var expectedByteArrays = inputs.Select(input => subject.Shorten(input).ToArray()).ToList();
        var threads = new Thread[threadCount];
        var exceptions = new ConcurrentBag<Exception>();

        // Act and Assert
        var action = () =>
        {
            try
            {
                for (var i = 0; i < iterationsPerThread; i++)
                {
                    var categoryIndex = i % categoryCount;

                    var actual = subject.Shorten(inputs[categoryIndex]).ToArray();
                    var expectedBytes = expectedByteArrays[categoryIndex];

                    Assert.Same(expectedBytes, actual);
                }
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
        };

        for (var i = 0; i < threadCount; i++)
        {
            threads[i] = new Thread(new ThreadStart(action));
            threads[i].Start();
        }

        Array.ForEach(threads, t => t.Join());

        if (exceptions.Count > 0)
        {
            throw new AggregateException(exceptions);
        }
    }
}
