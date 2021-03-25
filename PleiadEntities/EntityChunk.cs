using PleiadMisc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace PleiadEntities
{
    [Serializable]
    /// <summary>
    /// Container storing entities of the same type
    /// </summary>
    public class EntityChunk<T> : IEntityChunk, ISerializable
    {
        /// <summary>
        /// Container storing entities of the same type
        /// </summary>
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
        public EntityChunk(SerializationInfo info, StreamingContext context)
        {
            ChunkID = (int)info.GetValue("chunkId", typeof(int));
            string typeName = (string)info.GetValue("chunkType", typeof(string));
            var a = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name == "Pleiad").First();
            ChunkType = a.GetType(typeName);
            EntityCount = (int)info.GetValue("entityCount", typeof(int));
            Capacity = (int)info.GetValue("capacity", typeof(int));
            IsFull = (bool)info.GetValue("isFull", typeof(bool));
            _enitityIDs = (List<int>)info.GetValue("entityIDs", typeof(List<int>));
            _componentData = (List<IPleiadComponent>)info.GetValue("componentData", typeof(List<T>));
            _freeIndices = (Stack<int>)info.GetValue("freeIndices", typeof(Stack<int>));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("chunkId", ChunkID, typeof(int));
            info.AddValue("chunkType", ChunkType.FullName, typeof(string));
            info.AddValue("entityCount", EntityCount, typeof(int));
            info.AddValue("capacity", Capacity, typeof(int));
            info.AddValue("isFull", IsFull, typeof(bool));
            info.AddValue("entityIDs", _enitityIDs, typeof(List<int>));
            info.AddValue("componentData", _componentData, typeof(List<T>));
            info.AddValue("freeIndices", _freeIndices, typeof(Stack<int>));
        }

        public int ChunkID { get; private set; }
        public Type ChunkType { get; private set; }
        public int EntityCount { get; private set; }
        public int Capacity { get; private set; }
        public bool IsFull { get; private set; }
        public List<int> ChunkIDs => _enitityIDs;

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

                EntityCount++;
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
                EntityCount--;
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


        private readonly List<int> _enitityIDs;
        private List<IPleiadComponent> _componentData;
        private readonly Stack<int> _freeIndices;




#if DEBUG
        #region
        public void DEBUG_PrintEntities()
        {
            Console.WriteLine("---------------------");
            Console.WriteLine($"Chunk {ChunkID}");
            Console.WriteLine($"Type: {ChunkType}");
            Console.WriteLine($"Entities [{EntityCount}]:");
            for (int i = 0; i < _enitityIDs.Count; i++)
            {
                Console.WriteLine($"- Entity Index {i}, ID{_enitityIDs[i]}");
            }
            Console.WriteLine("---------------------");
        }

        #endregion
#endif
    }
}
