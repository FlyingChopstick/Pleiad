using Pleiad.Entities;


/// <summary>
/// Payload is used to set the data for the chunk
/// </summary>
/// <typeparam name="T"></typeparam>
public struct Payload<T> where T : IPleiadComponent
{
    public Payload(int chunkIndex, IPleiadComponent[] data)
    {
        ChunkIndex = chunkIndex;
        Data = data;
    }
    public Payload(int chunkIndex, T[] data)
    {
        ChunkIndex = chunkIndex;
        Data = new IPleiadComponent[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            Data[i] = data[i];
        }
    }

    public int ChunkIndex { get; }
    public IPleiadComponent[] Data;
}