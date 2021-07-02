using System;
using System.Collections.Generic;
using Pleiad.Entities.Model;

namespace Pleiad.Entities
{
    internal class EntityChunk
    {
        public EntityChunk(ChunkIndex chunkIndex, int chunkSize, Type chunkType)
        {
            ChunkIndex = chunkIndex;
            Capacity = chunkSize;
            EntityCount = 0;
            ChunkType = chunkType;
            IsFull = false;

            Entities = new(chunkSize);
            _componentData = new(chunkSize);
        }
        public int AddEntity(Entity entity, IPleiadComponent componentData)
        {
            int chunkId = -1;
            for (int i = 0; i < Entities.Count; i++)
            {
                if (Entities[i] == entity)
                {
                    chunkId = i;
                }
            }

            if (chunkId != -1)
            {
                _componentData[chunkId] = componentData;
                return chunkId;
            }

            EntityCount++;
            Entities.Add(entity);
            _componentData.Add(componentData);
            IsFull = EntityCount >= Capacity;
            return Entities.IndexOf(entity);
        }

        public bool RemoveEntity(Entity entity)
        {
            if (ContainsEntity(entity))
            {
                int index = Entities.IndexOf(entity);
                Entities.RemoveAt(index);
                _componentData.RemoveAt(index);
                return true;
            }

            return false;
        }

        public void SetComponentData(Entity entity, IPleiadComponent componentData)
        {
            if (ContainsEntity(entity))
            {
                int index = Entities.IndexOf(entity);
                _componentData[index] = componentData;
            }
            else
            {
                AddEntity(entity, componentData);
            }
        }
        public T GetComponentData<T>(Entity entity) where T : IPleiadComponent
        {
            if (ContainsEntity(entity))
            {
                int index = Entities.IndexOf(entity);

                IPleiadComponent temp = _componentData[index];

                return (T)Convert.ChangeType(temp, ChunkType);
            }
            return default;
        }

        public List<IPleiadComponent> GetAllData()
        {
            //List<T> output = new List<T>(_componentData.Count);
            //foreach (var data in _componentData)
            //{
            //    output.Add((T)Convert.ChangeType(data, ChunkType));
            //}
            return _componentData;
        }
        public void SetAllData<T>(T[] newData) where T : IPleiadComponent
        {
            if (newData.Length == _componentData.Count)
            {
                for (int i = 0; i < _componentData.Count; i++)
                {
                    _componentData[i] = newData[i];
                }
            }
            else
                throw new ArgumentException($"Can't set all: new data size is not equal to Entity count in chunk {ChunkType}:{ChunkIndex}");
        }
        public void SetDataAt<T>(T[] newData, int start = 0) where T : IPleiadComponent
        {
            if (newData.Length != 0)
            {
                for (int i = start; i < _componentData.Count; i++)
                {
                    _componentData[i] = newData[i];
                }
            }
            //if (newData.Length == _componentData.Count)
            //{

            //}
            //else
            //    throw new ArgumentException($"Can't set all: new data size is not equal to Entity count in chunk {ChunkType}:{ChunkIndex}");
        }

        public bool ContainsEntity(Entity entity) => Entities.Contains(entity);



        public Type ChunkType { get; }
        public ChunkIndex ChunkIndex { get; private set; }
        public int EntityCount { get; private set; }
        public int Capacity { get; private set; }
        public bool IsFull { get; private set; }
        public List<Entity> Entities { get; private set; }
        private readonly List<IPleiadComponent> _componentData;

        #region DebugFunctions
#if DEBUG
        public void DEBUG_PrintEntities()
        {
            Console.WriteLine("---------------------");
            Console.WriteLine($"Chunk {ChunkIndex}");
            Console.WriteLine($"Type: {ChunkType}");
            Console.WriteLine("Entities:");
            for (int i = 0; i < Entities.Count; i++)
            {
                Console.WriteLine($"- Entity Index {i}, ID{Entities[i]}");
            }
            Console.WriteLine("---------------------");
        }
#endif
        #endregion
    }
}
