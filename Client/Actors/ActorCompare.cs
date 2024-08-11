namespace Client.Actors;

public class ActorCompare<TKey> : IComparer<IActor>
{
    public int Compare(IActor? x, IActor? y)
    {
        switch (x)
        {
            case null when y == null:
                return -2; 
            case null:
                return -2;
        }

        if (y == null) return 1;

        if (x.Layer < y.Layer)
            return -1;
        if (x.Layer == y.Layer)
            return x.Z < y.Z ? -1 : 1;
        return 1;
    }
}