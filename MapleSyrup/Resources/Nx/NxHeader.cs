namespace MapleSyrup.Resources.Nx;

public record struct NxHeader
{
    private int magic;
    private int nodeCount;
    private long nodeOffset;
    private int stringCount;
    private long stringTableOffset;
    private int bitmapCount;
    private long bitmapTableOffset;
    private int audioCount;
    private long audioTableOffset;

    public NxHeader(int nCount, long baseOffset, int sCount, long sTableOffset,
        int bCount, long bTableoffset, int aCount, long aTableOffset)
    {
        nodeCount = nCount;
        nodeOffset = baseOffset;
        stringCount = sCount;
        stringTableOffset = sTableOffset;
        bitmapCount = bCount;
        bitmapTableOffset = bTableoffset;
        audioCount = aCount;
        audioTableOffset = aTableOffset;
    }

    public int GetMagic() => magic;

    public int GetNodeCount() => nodeCount;
    public int GetStringCount() => stringCount;
    public int GetBitmapCount() => bitmapCount;
    public int GetAudioCount() => audioCount;

    public long GetNodeOffset() => nodeOffset;
    public long GetStringOffsetTableOffset() => stringTableOffset;
    public long GetBitmapOffsetTableOffset() => bitmapTableOffset;
    public long GetAudioTableOffset() => audioTableOffset;
}