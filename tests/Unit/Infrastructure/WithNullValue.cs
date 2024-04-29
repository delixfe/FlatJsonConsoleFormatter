using System.Collections;

namespace Unit.Infrastructure;

public class WithNullValue : IReadOnlyList<KeyValuePair<string, object>>
{
    private readonly string _key;

    public WithNullValue(string key)
    {
        _key = key;
    }

    int IReadOnlyCollection<KeyValuePair<string, object>>.Count { get; } = 1;

    KeyValuePair<string, object> IReadOnlyList<KeyValuePair<string, object>>.this[int index]
    {
        get
        {
            if (index == 0)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new KeyValuePair<string, object>(_key, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }

            throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        yield return new KeyValuePair<string, object>(_key, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<string, object>>)this).GetEnumerator();
}
