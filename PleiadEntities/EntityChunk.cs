using PleiadEntities.Tools;
using PleiadMisc;
using System;
using System.Collections.Generic;

namespace PleiadEntities
{
    [Serializable]
    /// <summary>
    /// Container storing entities of the same type
    /// </summary>
    public class EntityChunk : IEntityChunk
    {
        private EntityChunk()
        {
        }

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

            ChunkIDs = new List<int>(capacity);
            ComponentData = new List<IPleiadComponent>(capacity);
            _freeIndices = new Stack<int>();

            for (int i = 0; i < capacity; i++)
            {
                ChunkIDs.Add(-1);
                ComponentData.Add(default);
                _freeIndices.Push(capacity - 1 - i);
            }
        }

        public EntityChunk(ChunkSaveObject save)
        {
            ChunkID = save.ChunkID;
            Capacity = save.Capacity;
            EntityCount = save.EntityCount;
            IsFull = EntityCount >= Capacity;
            ChunkIDs = save.IDs;
            ComponentData = save.ChunkData;
            ChunkType = save.ChunkType;

            _freeIndices = new(Capacity);
            for (int i = ChunkIDs.Count - 1; i >= 0; i--)
            {
                if (ChunkIDs[i] == -1)
                {
                    _freeIndices.Push(i);
                }
            }
        }
        //protected EntityChunk(SerializationInfo info, StreamingContext context)
        //{
        //    ChunkID = (int)info.GetValue("chunkId", typeof(int));
        //    string typeName = (string)info.GetValue("chunkType", typeof(string));
        //    var a = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name == "Pleiad").First();
        //    _chunkType = a.GetType(typeName);

        //    if (ChunkType is null)
        //    {
        //        throw new SerializationException("Could not get chunk type on deserialisation");
        //    }

        //    EntityCount = (int)info.GetValue("entityCount", typeof(int));
        //    Capacity = (int)info.GetValue("capacity", typeof(int));
        //    IsFull = (bool)info.GetValue("isFull", typeof(bool));
        //    ChunkIDs = (List<int>)info.GetValue("entityIDs", typeof(List<int>));

        //    _componentData = (List<IPleiadComponent>)info.GetValue("componentData", typeof(List<IPleiadComponent>));
        //    _freeIndices = (Stack<int>)info.GetValue("freeIndices", typeof(Stack<int>));
        //}
        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("chunkId", ChunkID, typeof(int));
        //    //info.AddValue("chunkType", ChunkType.FullName, typeof(string));
        //    info.AddValue("entityCount", EntityCount, typeof(int));
        //    info.AddValue("capacity", Capacity, typeof(int));
        //    info.AddValue("isFull", IsFull, typeof(bool));
        //    info.AddValue("entityIDs", ChunkIDs, typeof(List<int>));
        //    info.AddValue("componentData", _componentData, typeof(List<IPleiadComponent>));
        //    info.AddValue("freeIndices", _freeIndices, typeof(Stack<int>));
        //}

        public int ChunkID { get; private set; }

        //[NonSerialized]
        //private Type _chunkType;
        public Type ChunkType { get; private set; }
        public int EntityCount { get; private set; }
        public int Capacity { get; private set; }
        public bool IsFull { get; private set; }

        public List<IPleiadComponent> ComponentData { get; private set; }

        public List<int> ChunkIDs { get; private set; }// => _enitityIDs;

        public int AddEntity(int entityID, IPleiadComponent componentData)
        {
            if (_freeIndices.Count > 0)
            {
                int slot = _freeIndices.Pop();
                ChunkIDs[slot] = entityID;
                ComponentData[slot] = componentData;

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
                ChunkIDs[slot] = entityID;
                if (_freeIndices.Count == 0)
                    IsFull = true;

                EntityCount++;
                return slot;
            }
            else
                return -1;
        }

        public bool ContainsID(int entityID) => ChunkIDs.Contains(entityID);

        public Result<IPleiadComponent> GetComponentData(int entityID)
        {
            int slot = ChunkIDs.IndexOf(entityID);
            if (slot != -1)
            {
                return Result<IPleiadComponent>.Found(ComponentData[slot]);
            }

            return Result<IPleiadComponent>.NotFound;
        }

        public bool RemoveEntity(int entityID)
        {
            int slot = ChunkIDs.IndexOf(entityID);
            if (slot != -1)
            {
                ChunkIDs[slot] = -1;
                ComponentData[slot] = default;
                EntityCount--;
            }

            return false;
        }

        public bool SetComponentData(int entityID, IPleiadComponent componentData)
        {
            int slot = ChunkIDs.IndexOf(entityID);
            if (slot == -1)
            {
                if (_freeIndices.Count > 0)
                {
                    slot = _freeIndices.Pop();
                    ChunkIDs[slot] = entityID;
                }
                else
                    return false;
            }

            ComponentData[slot] = componentData;
            return true;
        }

        public List<IPleiadComponent> GetChunkData() => ComponentData;
        public List<object> DumpChunkData()
        {
            Type dataType = ChunkType;
            List<object> dump = new List<object>(ComponentData.Count);

            for (int i = 0; i < ComponentData.Count; i++)
            {
                if (ComponentData[i] is not null)
                {
                    dump.Add(ComponentData[i]);//(object)Convert.ChangeType((object)_componentData[i], dataType));
                }
                else
                {
                    dump.Add(default);
                }
            }

            return dump;
        }

        public void SetChunkData<T>(List<T> data) where T : IPleiadComponent
        {
            for (int i = 0; i < data.Count; i++)
            {
                ComponentData[i] = data[i];
            }
        }
        public void SetChunkData<T>(T[] data) where T : IPleiadComponent
        {
            for (int i = 0; i < ComponentData.Capacity; i++)
            {
                if (data[i] != null)
                {
                    ComponentData[i] = data[i];
                }
            }
        }
        public void SetChunkIDs(List<int> ids)
        {
            ChunkIDs = ids;
        }

        //private readonly List<int> ChunkIDs;
        //private List<IPleiadComponent> ComponentData;
        private readonly Stack<int> _freeIndices;

#if DEBUG
        #region
        public void DEBUG_PrintEntities()
        {
            Console.WriteLine("---------------------");
            Console.WriteLine($"Chunk {ChunkID}");
            Console.WriteLine($"Type: {ChunkType}");
            Console.WriteLine($"Entities [{EntityCount}]:");
            for (int i = 0; i < ChunkIDs.Count; i++)
            {
                Console.WriteLine($"- Entity Index {i}, ID{ChunkIDs[i]}");
            }
            Console.WriteLine("---------------------");
        }


        #endregion
#endif
    }
}
