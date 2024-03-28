namespace MapleSyrup.Utilities;

public class VariantMap<TK1, TK2, T> : IDisposable
{
    private Dictionary<(TK1, TK2), T> container;

    public int FirstKeyCount => container.Keys.Count;

    public VariantMap()
    {
        container = new ();
    }

    public T this[TK1 primaryKey, TK2 secondaryKey]
    {
        get => container[(primaryKey, secondaryKey)];
        set => container[(primaryKey, secondaryKey)] = value;
    }

    public bool Contains(TK1 pk, TK2 sk)
    {
        if (container.ContainsKey((pk, sk)))
            return true;
        return false;
    }

    public void Clear()
    {
        container.Clear();
    }

    public void Dispose()
    {
        Clear();
    }
}