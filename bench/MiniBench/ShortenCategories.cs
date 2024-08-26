using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using JsonConsoleFormatters.CategoryShortening;

namespace Benchmarks.MiniBench;

public class ShortenCategories
{
    private const int CategoryCount = 100;
    private const int Iterations = 100;
    private const int OpsPerInvoke = CategoryCount * Iterations;

    private readonly IEnumerable<KeyValuePair<string, string>> _mappings =
    [
        new KeyValuePair<string, string>("Company.Service.Product.", "Product."),
        new KeyValuePair<string, string>("Microsoft.Extensions.", "MSE."),
        new KeyValuePair<string, string>("Microsoft.AspNetCore.", "AspNet.")
    ];

    private ICategoryShortener? _cachingCategoryShortener;


    private string[]? _categories;

    private ICategoryShortener? _renameCategoryShortener;
    private ICategoryShortener? _stringReplaceCategoryShortener;
    private ICategoryShortener? _utf8ConvertingCategoryShortener;

    private Writer _writer;

    [GlobalSetup]
    public void Setup() => _categories = CreateTestCategories(CategoryCount).ToArray();

    private static IEnumerable<string> CreateTestCategories(int amount)
    {
        var count = amount / 4;
        for (var i = 0; i < count; i++)
        {
            yield return $"Company.Service.Product.Class.Method.N{i:0000}";
            yield return $"Microsoft.Extensions.FileSystemGlobbing.N{i:0000}";
            yield return $"Microsoft.AspNetCore.Hosting.WebHostBuilder.N{i:0000}";
            yield return $"Aspire.RabbitMQ.Client.RabbitMQEventSourceLogForwarder.N{i:0000}";
        }
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _renameCategoryShortener = new RenameCategoryShortener(_mappings);
        _cachingCategoryShortener = new CachingCategoryShortener(_renameCategoryShortener!);
        _stringReplaceCategoryShortener = new StringReplaceCategoryShortener(_mappings);
        _utf8ConvertingCategoryShortener = new Utf8ConvertingCategoryShortener();
        _writer = new Writer();
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        _renameCategoryShortener = null;
        _cachingCategoryShortener = null;
        _stringReplaceCategoryShortener = null;
        _utf8ConvertingCategoryShortener = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long Shorten(ICategoryShortener shortener, string[] categories, Writer writer)
    {
        for (var j = 0; j < Iterations; j++)
        for (var i = 0; i < categories!.Length; i++)
        {
            writer.Write(shortener.Shorten(categories[i]));
        }

        return writer.Written;
    }

    [Benchmark(OperationsPerInvoke = OpsPerInvoke, Baseline = true)]
    public long RenameCategoryShortener() => Shorten(_renameCategoryShortener!, _categories!, _writer);

    [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
    public long CachingRenameCategoryShortener() => Shorten(_cachingCategoryShortener!, _categories!, _writer);

    [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
    public long StringReplaceRenameCategoryShortener() =>
        Shorten(_stringReplaceCategoryShortener!, _categories!, _writer);

    [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
    public long NonShorteningUtf8ConvertingCategoryShortener() =>
        Shorten(_utf8ConvertingCategoryShortener!, _categories!, _writer);

    private struct Writer
    {
        public long Written;

        public void Write(ReadOnlySpan<byte> span) => Written += span.Length;
    }

    private class StringReplaceCategoryShortener : ICategoryShortener
    {
        private readonly ImmutableDictionary<string, string> _config;
        private readonly Encoding _encoding = new UTF8Encoding(false);

        public StringReplaceCategoryShortener(IEnumerable<KeyValuePair<string, string>> mappings)
        {
            _config = mappings.ToImmutableDictionary();
        }

        public ReadOnlySpan<byte> Shorten(string category)
        {
            if (category.Length == 0)
            {
                return ReadOnlySpan<byte>.Empty;
            }

            foreach (var (source, target) in _config)
            {
                if (category.StartsWith(source))
                {
                    var replaced = category.Replace(source, target);

                    return _encoding.GetBytes(replaced);
                }
            }

            return _encoding.GetBytes(category);
        }
    }
}
