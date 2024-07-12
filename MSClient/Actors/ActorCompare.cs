namespace MSClient.Actors;

public class ActorCompare<TKey> : IComparer<Actor>
{
    public int Compare(Actor? x, Actor? y)
    {
        if (x == null && y == null) return 0;
        if (x == null) return -1;
        if (y == null) return 1;

        if (x.Z < y.Z)
            return -1;
        if (x.Z > y.Z)
            return 1;
        return 0;
    }
}