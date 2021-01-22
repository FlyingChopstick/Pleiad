using PleiadMisc;
using System;
using System.Collections.Generic;

namespace PleiadEntities
{
    public class EntityChunk : IEntityChunk
    {
        public EntityChunk(int chunkID, Type chunkType, int capacity)
        {
            ChunkID = chunkID;
            ChunkType = chunkType;
            Capacity = capacity;
            EntityCount = 0;
            IsFull = false;

            _enitityIDs = new List<int>(capacity);
            _componentData = new List<IPleiadComponent>(capacity);
            _freeIndices = new Stack<int>();

            for (int i = 0; i < capacity; i++)
            {
                _enitityIDs.Add(-1);
                _componentData.Add(default);
                _freeIndices.Push(capacity - 1 - i);
            }
        }

        public int ChunkID { get; private set; }
        public Type ChunkType { get; private set; }
        public int EntityCount { get; private set; }
        public int Capacity { get; private set; }
        public bool IsFull { get; private set; }
        public IEnumerable<int> ChunkIDs => _enitityIDs;

        public int AddEntity(int entityID, IPleiadComponent componentData)
        {
            if (_freeIndices.Count > 0)
            {
                int slot = _freeIndices.Pop();
                _enitityIDs[slot] = entityID;
                _componentData[slot] = componentData;

                if (_freeIndices.Count == 0)
                    IsFull = true;

                return slot;
            }
            else
                return -1;
        }
        public int AddEntity(int entityID)
        {
            if (_freeIndices.Count > 0)
            {
                int slot = _freeIndices.Pop();
                _enitityIDs[slot] = entityID;
                if (_freeIndices.Count == 0)
                    IsFull = true;

                return slot;
            }
            else
                return -1;
        }

        public bool ContainsID(int entityID) => _enitityIDs.Contains(entityID);

        public Result<IPleiadComponent> GetComponentData(int entityID)
        {
            int slot = _enitityIDs.IndexOf(entityID);
            if (slot != -1)
            {
                return Result<IPleiadComponent>.Found(_componentData[slot]);
            }

            return Result<IPleiadComponent>.NotFound;
        }

        public bool RemoveEntity(int entityID)
        {
            int slot = _enitityIDs.IndexOf(entityID);
            if (slot != -1)
            {
                _enitityIDs[slot] = -1;
                _componentData[slot] = default;
            }

            return false;
        }

        public bool SetComponentData(int entityID, IPleiadComponent componentData)
        {
            int slot = _enitityIDs.IndexOf(entityID);
            if (slot == -1)
            {
                if (_freeIndices.Count > 0)
                {
                    slot = _freeIndices.Pop();
                }
                else
                    return false;
            }

            _componentData[slot] = componentData;
            return true;
        }

        public List<IPleiadComponent> GetChunkData() => _componentData;
        public void SetChunkData<T>(List<T> data) where T : IPleiadComponent
        {
            for (int i = 0; i < data.Count; i++)
            {
                _componentData[i] = data[i];
            }
        }
        public void SetChunkData<T>(T[] data) where T : IPleiadComponent
        {
            for (int i = 0; i < _componentData.Capacity; i++)
            {
                if (data[i] != null)
                {
                    _componentData[i] = data[i];
                }
            }
        }

        public void DEBUG_PrintEntities()
        {
            Console.WriteLine("---------------------");
            Console.WriteLine($"Chunk {ChunkID}");
            Console.WriteLine($"Type: {ChunkType}");
            Console.WriteLine("Entities:");
            for (int i = 0; i < _enitityIDs.Count; i++)
            {
                Console.WriteLine($"- Entity Index {i}, ID{_enitityIDs[i]}");
            }
            Console.WriteLine("---------------------");
        }

        private readonly List<int> _enitityIDs;
        private List<IPleiadComponent> _componentData;
        private readonly Stack<int> _freeIndices;
    }






    //    internal class EntityChunk
    //    {
    //        public EntityChunk(int chunkIndex, int chunkSize, Type chunkType)
    //        {
    //            ChunkIndex = chunkIndex;
    //            Capacity = chunkSize;
    //            EntityCount = 0;
    //            ChunkType = chunkType;
    //            IsFull = false;

    //            ChunkIDs = new List<int>(chunkSize);
    //            _componentData = new List<IPleiadComponent>(chunkSize);
    //        }
    //        public int AddEntity(int entityID, IPleiadComponent componentData)
    //        {
    //            if (!ChunkIDs.Contains(entityID))
    //            {
    //                EntityCount++;
    //                ChunkIDs.Add(entityID);
    //                _componentData.Add(componentData);
    //                IsFull = EntityCount >= Capacity;
    //                return ChunkIDs.IndexOf(entityID);
    //            }
    //            return -1;
    //        }

    //        public void RemoveEntity(int entityID)
    //        {
    //            if (IsInChunk(entityID))
    //            {
    //                int index = ChunkIDs.IndexOf(entityID);
    //                ChunkIDs.RemoveAt(index);
    //                _componentData.RemoveAt(index);
    //            }
    //        }

    //        public void SetComponentData(int entityID, IPleiadComponent componentData)
    //        {
    //            if (IsInChunk(entityID))
    //            {
    //                int index = ChunkIDs.IndexOf(entityID);
    //                _componentData[index] = componentData;
    //            }
    //            else
    //            {
    //                AddEntity(entityID, componentData);
    //            }
    //        }
    //        public T GetComponentData<T>(Entity entity) where T : IPleiadComponent
    //        {
    //            if (IsInChunk(entity.ID))
    //            {
    //                int index = ChunkIDs.IndexOf(entity.ID);

    //                var t =  _componentData[index].GetType();
    //                if (t == ChunkType)
    //                {
    //                    T temp = (T)_componentData[index];

    //                    return (T)Convert.ChangeType(temp, ChunkType);
    //                }
    //            }
    //            return default;
    //        }

    //        public List<IPleiadComponent> GetAllData()
    //        {
    //            //List<T> output = new List<T>(_componentData.Count);
    //            //foreach (var data in _componentData)
    //            //{
    //            //    output.Add((T)Convert.ChangeType(data, ChunkType));
    //            //}
    //            return _componentData;
    //        }
    //        public void SetAllData<T>(T[] newData) where T : IPleiadComponent
    //        {
    //            if (newData.Length == _componentData.Count)
    //            {
    //                for (int i = 0; i < _componentData.Count; i++)
    //                {
    //                    _componentData[i] = newData[i];
    //                }
    //            }
    //            else
    //                throw new ArgumentException($"Can't set all: new data size is not equal to Entity count in chunk {ChunkType}:{ChunkIndex}");
    //        }
    //        public void SetDataAt<T>(T[] newData, int start = 0) where T : IPleiadComponent
    //        {
    //            if (newData.Length != 0)
    //            {
    //                for (int i = start; i < _componentData.Count; i++)
    //                {
    //                    _componentData[i] = newData[i];
    //                }
    //            }
    //            //if (newData.Length == _componentData.Count)
    //            //{

    //            //}
    //            //else
    //            //    throw new ArgumentException($"Can't set all: new data size is not equal to Entity count in chunk {ChunkType}:{ChunkIndex}");
    //        }

    //        public bool IsInChunk(Entity entity) => ChunkIDs.Contains(entity.ID);
    //        public bool IsInChunk(int entityID) => ChunkIDs.Contains(entityID);



    //        public Type ChunkType { get; }
    //        public int ChunkIndex { get; private set; }
    //        public int EntityCount { get; private set; }
    //        public int Capacity { get; private set; }
    //        public bool IsFull { get; private set; }
    //        public List<int> ChunkIDs { get; private set; }
    //        private readonly List<IPleiadComponent> _componentData;

    //        #region DebugFunctions
    //#if DEBUG
    //        public void DEBUG_PrintEntities()
    //        {
    //            Console.WriteLine("---------------------");
    //            Console.WriteLine($"Chunk {ChunkIndex}");
    //            Console.WriteLine($"Type: {ChunkType}");
    //            Console.WriteLine("Entities:");
    //            for (int i = 0; i < ChunkIDs.Count; i++)
    //            {
    //                Console.WriteLine($"- Entity Index {i}, ID{ChunkIDs[i]}");
    //            }
    //            Console.WriteLine("---------------------");
    //        }
    //#endif
    //        #endregion

    //        #region Overrides
    //        public override bool Equals(object obj)
    //        {
    //            return obj is EntityChunk chunk &&
    //                   EqualityComparer<Type>.Default.Equals(ChunkType, chunk.ChunkType) &&
    //                   ChunkIndex == chunk.ChunkIndex &&
    //                   EntityCount == chunk.EntityCount &&
    //                   Capacity == chunk.Capacity &&
    //                   IsFull == chunk.IsFull &&
    //                   EqualityComparer<List<int>>.Default.Equals(ChunkIDs, chunk.ChunkIDs) &&
    //                   EqualityComparer<List<IPleiadComponent>>.Default.Equals(_componentData, chunk._componentData);
    //        }

    //        public override int GetHashCode()
    //        {
    //            int hashCode = -397465017;
    //            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(ChunkType);
    //            hashCode = hashCode * -1521134295 + ChunkIndex.GetHashCode();
    //            hashCode = hashCode * -1521134295 + EntityCount.GetHashCode();
    //            hashCode = hashCode * -1521134295 + Capacity.GetHashCode();
    //            hashCode = hashCode * -1521134295 + IsFull.GetHashCode();
    //            hashCode = hashCode * -1521134295 + EqualityComparer<List<int>>.Default.GetHashCode(ChunkIDs);
    //            hashCode = hashCode * -1521134295 + EqualityComparer<List<IPleiadComponent>>.Default.GetHashCode(_componentData);
    //            return hashCode;
    //        }
    //        #endregion
    //    }
}
