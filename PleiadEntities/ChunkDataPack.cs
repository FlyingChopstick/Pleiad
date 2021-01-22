using PleiadEntities;
using PleiadMisc;
using System;
using System.Collections.Generic;

/// <summary>
/// Datapack is used to get the data from the chunks
/// </summary>
/// <typeparam name="T"></typeparam>
public struct ChunkDataPack
{
    //stores raw data from chunks

    private Dictionary<int, List<IPleiadComponent>> _storage;
    public Type DataType { get; private set; }
    public void StoreChunk(int chunkIndex, List<IPleiadComponent> chunkData)
    {
        _storage.Add(chunkIndex, chunkData);
    }
    public void Flush() => _storage.Clear();
    public Dictionary<int, List<IPleiadComponent>> Pack { get => _storage; }

    public ChunkDataPack(Type dataType)
    {
        DataType = dataType;
        _storage = new Dictionary<int, List<IPleiadComponent>>();
    }

    public Result<List<IPleiadComponent>> GetRawData(int chunkIndex)
    {
        if (_storage.ContainsKey(chunkIndex))
        {
            return Result<List<IPleiadComponent>>.Found(_storage[chunkIndex]);
        }
        else
            return Result<List<IPleiadComponent>>.NotFound;
    }
    public Result<List<T>> GetConvertedData<T>(int chunkIndex)
    {
        if (_storage.ContainsKey(chunkIndex))
        {
            List<T> res = new List<T>(_storage[chunkIndex].Count);
            for (int i = 0; i < _storage[chunkIndex].Count; i++)
            {
                res[i] = (T)Convert.ChangeType((T)_storage[chunkIndex][i], DataType);
            }
            return Result<List<T>>.Found(res);
        }
        else
            return Result<List<T>>.NotFound;
    }



    //public DataPack(int[] indices, Dictionary<int, int> chunkSizes, IPleiadComponent[] data)
    //{
    //    _chunkIndices = indices;
    //    ChunkSizes = chunkSizes;
    //    _dataRaw = data;
    //}
    //public DataPack(int[] indices, Dictionary<int, int> chunkSizes, T[] data)
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
    //public T[] GetConvertedData()
    //{
    //    T[] arr = new T[_dataRaw.Length];
    //    for (int i = 0; i < _dataRaw.Length; i++)
    //    {
    //        arr[i] = (T)System.Convert.ChangeType(_dataRaw[i], typeof(T));
    //    }
    //    return arr;
    //}
    //Returns the data as IPleiadComponent[]
    //public IPleiadComponent[] GetRawData()
    //{
    //    return _dataRaw;
    //}
    //public int[] GetChunkIndices() { return _chunkIndices; }
    //public readonly Dictionary<int, int> ChunkSizes { get; }

    //private readonly int[] _chunkIndices;
    //private readonly IPleiadComponent[] _dataRaw;
}