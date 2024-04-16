using System.Collections;
using MapleSyrup.GameObjects;

namespace MapleSyrup.Utils;

public class SafeSortedSet<T> : IEnumerable<T> where T : IComparable<T>
{
    private readonly SortedSet<T> _set = new();
    private readonly object _mutexLock = new();

    public void Add(T item)
    {
        lock (_mutexLock)
        {
            _set.Add(item);
        }
    }
    
    public bool Contains(T item)
    {
        lock (_mutexLock)
        {
            return _set.Contains(item);
        }
    }
    
    public void Remove(T item)
    {
        lock (_mutexLock)
        {
            _set.Remove(item);
        }
    }
    
    public IEnumerable<T> GetItems()
    {
        lock (_mutexLock)
        {
            return _set.ToList();
        }
    }
    
    public int Count()
    {
        lock (_mutexLock)
        {
            return _set.Count;
        }
    }
    
    public void Clear()
    {
        lock (_mutexLock)
        {
            _set.Clear();
        }
    }
    
    public bool IsEmpty()
    {
        lock (_mutexLock)
        {
            return _set.Count == 0;
        }
    }
    
    public IEnumerator<T> GetEnumerator()
    {
        lock (_mutexLock)
        {
            return _set.GetEnumerator();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}