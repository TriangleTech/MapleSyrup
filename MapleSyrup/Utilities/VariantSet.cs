namespace MapleSyrup.Utilities;

public class VariantSet<T> : IDisposable
{
    private readonly Dictionary<string, Dictionary<string, T>> set;

    public VariantSet()
    {
        set = new Dictionary<string, Dictionary<string, T>>();
    }

    public Dictionary<string, T> this[string name]
    {
        get
        {
            if (set.TryGetValue(name, out var item))
                return item;
            set[name] = new Dictionary<string, T>();

            return set[name];
        }
    }

    public void ClearVariant(string stateName)
    {
        if (!set.ContainsKey(stateName))
            return;
        set[stateName].Clear();
    }

    public void ClearSet()
    {
        foreach (var state in set)
            set.Clear();
        set.Clear();
    }

    public void Dispose()
    {
        ClearSet();
    }
}