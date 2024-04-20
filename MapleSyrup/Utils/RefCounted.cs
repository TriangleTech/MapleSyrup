namespace MapleSyrup.Utils;

public class RefCounted<T>
{
    private int _count = 0;
    private T _value;

    public RefCounted(T obj)
    {
        _value = obj;
        _count = 1;
    }

    public T Get()
    {
        _count++;
        return _value;
    }

    public void Release(bool disposing = false)
    {
        if (_count <= 0) return;
        
        _count--;
        if (_count == 0 && _value is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        if (disposing && _value is IDisposable dispose)
        {
            dispose.Dispose();
        }
        
    }
}