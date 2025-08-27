using System.Buffers;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.dotMemory;
using JsonConsoleFormatters.CategoryShortening;

namespace Benchmarks.MiniBench;

// [DotTraceDiagnoser]
[DotMemoryDiagnoser]
// [UnrollFactor]
public class ShortenCategories
{
    private const int CategoryCount = 1000;
    private const int Iterations = 100;
    private const int OpsPerInvoke = CategoryCount * Iterations;

    private static readonly JsonWriterOptions _jsonWriterOptions = new();

    private static readonly BufferWriter _bufferWriter = new();


    private readonly IEnumerable<KeyValuePair<string, string>> _mappings =
    [
        new KeyValuePair<string, string>("Company.Service.Product.", "Product."),
        new KeyValuePair<string, string>("Microsoft.Extensions.", "MSE."),
        new KeyValuePair<string, string>("Microsoft.AspNetCore.", "AspNet.")
    ];

    private ICategoryShortener? _cachingCategoryShortener;


    private string[]? _categories;

    private ICategoryShortener? _renameCategoryShortener;
    private StringReplaceCategoryShortener? _stringReplaceCategoryShortener;
    private ICategoryShortener? _utf8ConvertingCategoryShortener;

    [GlobalSetup]
    public void Setup()
    {
        _categories = CreateTestCategories(CategoryCount).ToArray();
        //
    }


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
        _bufferWriter.Reset();
    }

    [IterationCleanup]
    public void IterationCleanup()
    {
        // _renameCategoryShortener = null;
        // _cachingCategoryShortener = null;
        // _stringReplaceCategoryShortener = null;
        // _utf8ConvertingCategoryShortener = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long Shorten(ICategoryShortener shortener, string[] categories, BufferWriter bufferWriter)
    {
        for (var j = 0; j < Iterations; j++)
        for (var i = 0; i < categories!.Length; i++)
        {
            using (var writer = new Utf8JsonWriter(bufferWriter, _jsonWriterOptions))
            {
                writer.WriteStartObject();
                writer.WriteString("logger"u8, shortener.Shorten(categories[i]));
                writer.WriteEndObject();
            }
        }

        return bufferWriter.Written;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long ShortenString(StringReplaceCategoryShortener shortener, string[] categories,
        BufferWriter bufferWriter)
    {
        for (var j = 0; j < Iterations; j++)
        for (var i = 0; i < categories!.Length; i++)
        {
            using (var writer = new Utf8JsonWriter(bufferWriter, _jsonWriterOptions))
            {
                writer.WriteStartObject();
                writer.WriteString("logger"u8, shortener.ShortenString(categories[i]));
                writer.WriteEndObject();
            }
        }

        return bufferWriter.Written;
    }

    [Benchmark(OperationsPerInvoke = OpsPerInvoke, Baseline = true)]
    public long RenameCategoryShortener() => Shorten(_renameCategoryShortener!, _categories!, _bufferWriter!);

    [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
    public long CachingRenameCategoryShortener() => Shorten(_cachingCategoryShortener!, _categories!, _bufferWriter!);

    [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
    public long StringReplaceRenameCategoryShortener() =>
        ShortenString(_stringReplaceCategoryShortener!, _categories!, _bufferWriter!);

    [Benchmark(OperationsPerInvoke = OpsPerInvoke)]
    public long NonShorteningUtf8ConvertingCategoryShortener() =>
        Shorten(_utf8ConvertingCategoryShortener!, _categories!, _bufferWriter!);

    private class BufferWriter : IBufferWriter<byte>
    {
        // public const int MaximumBufferSize = 0X7FFFFFC7;
        public const int MaximumBufferSize = 1024;

        private readonly byte[] _buffer = new byte[MaximumBufferSize];
        public long Written;

        public void Advance(int count) => Written += count;

        public Memory<byte> GetMemory(int sizeHint = 0) => _buffer.AsMemory();

        public Span<byte> GetSpan(int sizeHint = 0) => _buffer.AsSpan();

        public void Reset() => Written = 0;
    }


    private class StringReplaceCategoryShortener : ICategoryShortener
    {
        private readonly ImmutableDictionary<string, string> _config;
        private readonly Encoding _encoding = new UTF8Encoding(false);


        public StringReplaceCategoryShortener(IEnumerable<KeyValuePair<string, string>> mappings)
        {
            _config = mappings.ToImmutableDictionary();
        }

        public ReadOnlySpan<byte> Shorten(string category) => _encoding.GetBytes(ShortenString(category));

        public string ShortenString(string category)
        {
            if (category.Length == 0)
            {
                return category;
            }

            foreach (var (source, target) in _config)
            {
                if (category.StartsWith(source))
                {
                    return category.Replace(source, target);
                }
            }

            return category;
        }
    }
}
