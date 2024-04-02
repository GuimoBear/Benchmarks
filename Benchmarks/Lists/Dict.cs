namespace Benchmarks.Lists;

public sealed class Dict<TKey, TValue> where TKey : notnull
{
    private readonly IEqualityComparer<TKey> comparer;

    public Dict()
    {
        comparer = EqualityComparer<TKey>.Default;
    }
}
