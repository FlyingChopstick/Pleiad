using PleiadEntities;
using System;
using System.Collections.Generic;

/// <summary>
/// Datapack is used to get the data from the chunks
/// </summary>
/// <typeparam name="T"></typeparam>
public struct DataPack<T> where T : IPleiadComponent
{
    public DataPack(int[] indices, Dictionary<int, int> chunkSizes, IPleiadComponent[] data)
    {
        _chunkIndices = indices;
        ChunkSizes = chunkSizes;
        _dataRaw = data;
    }
    public DataPack(int[] indices, Dictionary<int, int> chunkSizes, T[] data)
    {
        _chunkIndices = indices;
        ChunkSizes = chunkSizes;
        _dataRaw = new IPleiadComponent[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            _dataRaw[i] = data[i];
        }
    }


    //Returns the data as T[]
    public T[] GetConvertedData()
    {
        T[] arr = new T[_dataRaw.Length];
        for (int i = 0; i < _dataRaw.Length; i++)
        {
            arr[i] = (T)System.Convert.ChangeType(_dataRaw[i], typeof(T));
        }
        return arr;
    }
    //Returns the data as IPleiadComponent[]
    public IPleiadComponent[] GetRawData()
    {
        return _dataRaw;
    }
    public int[] GetChunkIndices() { return _chunkIndices; }
    public readonly Dictionary<int, int> ChunkSizes { get; }

    private readonly int[] _chunkIndices;
    private readonly IPleiadComponent[] _dataRaw;
}