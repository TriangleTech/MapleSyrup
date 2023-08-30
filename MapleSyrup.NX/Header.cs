namespace MapleSyrup.NX;

public class Header
{
    private byte[] PKG4_MAGIC = { 0x5B, 0x4B, 0x47, 0x34 };

    private uint magic;
    private uint nodeCount;
    private ulong nodeOffset;
    private uint stringCount;
    private ulong stringTableOffset;
    private uint bitmapCount;
    private ulong bitmapTableOffset;
    private uint audioCount;
    private ulong audioTableOffset;

    public Header(uint nCount, ulong baseOffset, uint sCount, ulong sTableOffset,
        uint bCount, ulong bTableoffset, uint aCount, ulong aTableOffset)
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

    public uint GetMagic() => magic;

    public uint GetNodeCount() => nodeCount;
    public uint GetStringCount() => stringCount;
    public uint GetBitmapCount() => bitmapCount;
    public uint GetAudioCount() => audioCount;

    public ulong GetNodeOffset() => nodeOffset;
    public ulong GetStringOffsetTableOffset() => stringTableOffset;
    public ulong GetBitmapOffsetTableOffset() => bitmapTableOffset;
    public ulong GetAudioTableOffset() => audioTableOffset;
}