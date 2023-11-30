namespace MapleSyrup.Core.Event;

public enum DataType
{
    // Generic Value Types
    Byte,
    Short,
    Int,
    Long,
    Double,
    Float,
    String,
    
    // FNA Value Types
    GameTime,
    
    // Nx Value Types
    NxNode,
    NxAudioNode,
    NxBitmapNode,
    NxDoubleNode,
    NxIntNode,
    NxStringNode,
    NxVectorNode,
}