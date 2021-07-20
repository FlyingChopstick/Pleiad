using Pleiad.Entities.Components;

/// <summary>
/// Datapack is used to get the data from the chunks
/// </summary>
/// <typeparam name="T"></typeparam>
public struct DataPack<T> where T : IPleiadComponent
{
    public static DataPack<T> Empty => new() { Data = null };

    public DataPack(int size)
    {
        Data = new IPleiadComponent[size][];
    }

    public int Length => Data.Length;
    public IPleiadComponent[][] Data;

    //public DataPack(ChunkIndex[] indices, Dictionary<ChunkIndex, int> chunkSizes, IPleiadComponent[] data)
    //{
    //    _chunkIndices = indices;
    //    ChunkSizes = chunkSizes;
    //    _dataRaw = data;
    //}
    //public DataPack(ChunkIndex[] indices, Dictionary<ChunkIndex, int> chunkSizes, T[] data)
    //{
    //    _chunkIndices = indices;
    //    ChunkSizes = chunkSizes;
    //    _dataRaw = new IPleiadComponent[data.Length];
    //    for (int i = 0; i < data.Length; i++)
    //    {
    //        _dataRaw[i] = data[i];
    //    }
    //}


    //Returns the data as T[]
    public T[][] GetConvertedData()
    {
        T[][] arr = new T[Data.Length][];
        for (int i = 0; i < Data.Length; i++)
        {
            arr[i] = new T[Data[i].Length];
            for (int j = 0; j < Data[i].Length; j++)
            {
                arr[i][j] = (T)System.Convert.ChangeType(Data[i][j], typeof(T));
            }
        }
        return arr;
    }
    ////Returns the data as IPleiadComponent[]
    //public IPleiadComponent[] GetRawData()
    //{
    //    return _dataRaw;
    //}
    //public ChunkIndex[] GetChunkIndices() { return _chunkIndices; }
    //public readonly Dictionary<ChunkIndex, int> ChunkSizes { get; }

    //private readonly ChunkIndex[] _chunkIndices;
    //private readonly IPleiadComponent[] _dataRaw;
}