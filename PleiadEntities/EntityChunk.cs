using System;
using System.Collections.Generic;

namespace PleiadEntities
{
    internal class EntityChunk
    {
        public EntityChunk(int chunkIndex, int chunkSize, Type chunkType)
        {
            ChunkIndex = chunkIndex;
            Capacity = chunkSize;
            EntityCount = 0;
            ChunkType = chunkType;
            IsFull = false;

            ChunkIDs = new List<int>(chunkSize);
            _componentData = new List<IPleiadComponent>(chunkSize);
        }
        public int AddEntity(int entityID, IPleiadComponent componentData)
        {
            if (!ChunkIDs.Contains(entityID))
            {
                EntityCount++;
                ChunkIDs.Add(entityID);
                _componentData.Add(componentData);
                IsFull = EntityCount >= Capacity;
                return ChunkIDs.IndexOf(entityID);
            }
            return -1;
        }

        public void RemoveEntity(int entityID)
        {
            if (IsInChunk(entityID))
            {
                int index = ChunkIDs.IndexOf(entityID);
                ChunkIDs.RemoveAt(index);
                _componentData.RemoveAt(index);
            }
        }

        public void SetComponentData(int entityID, IPleiadComponent componentData)
        {
            if (IsInChunk(entityID))
            {
                int index = ChunkIDs.IndexOf(entityID);
                _componentData[index] = componentData;
            }
            else
            {
                AddEntity(entityID, componentData);
            }
        }
        public T GetComponentData<T>(Entity entity) where T : IPleiadComponent
        {
            if (IsInChunk(entity.ID))
            {
                int index = ChunkIDs.IndexOf(entity.ID);

                IPleiadComponent temp = _componentData[index];

                return (T)Convert.ChangeType(temp, ChunkType);
            }
            return default;
        }

        public ref List<T> GetAllData<T>()
        {
            List<T> output = new List<T>(_componentData.Count);
            foreach (var data in _componentData)
            {
                output.Add((T)Convert.ChangeType(data, ChunkType));
            }
            return output;
        }
        public void SetAll(IPleiadComponent[] newData)
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

        public bool IsInChunk(Entity entity) => ChunkIDs.Contains(entity.ID);
        public bool IsInChunk(int entityID) => ChunkIDs.Contains(entityID);

        public Type ChunkType { get; }
        public int ChunkIndex { get; private set; }
        public int EntityCount { get; private set; }
        public int Capacity { get; private set; }
        public bool IsFull { get; private set; }
        public List<int> ChunkIDs { get; private set; }
        private readonly List<IPleiadComponent> _componentData;

        #region DebugFunctions
#if DEBUG
        public void DEBUG_PrintEntities()
        {
            Console.WriteLine("---------------------");
            Console.WriteLine($"Chunk {ChunkIndex}");
            Console.WriteLine($"Type: {ChunkType}");
            Console.WriteLine("Entities:");
            for (int i = 0; i < ChunkIDs.Count; i++)
            {
                Console.WriteLine($"- Entity Index {i}, ID{ChunkIDs[i]}");
            }
            Console.WriteLine("---------------------");
        }
#endif
        #endregion

        #region Overrides
        public override bool Equals(object obj)
        {
            return obj is EntityChunk chunk &&
                   EqualityComparer<Type>.Default.Equals(ChunkType, chunk.ChunkType) &&
                   ChunkIndex == chunk.ChunkIndex &&
                   EntityCount == chunk.EntityCount &&
                   Capacity == chunk.Capacity &&
                   IsFull == chunk.IsFull &&
                   EqualityComparer<List<int>>.Default.Equals(ChunkIDs, chunk.ChunkIDs) &&
                   EqualityComparer<List<IPleiadComponent>>.Default.Equals(_componentData, chunk._componentData);
        }

        public override int GetHashCode()
        {
            int hashCode = -397465017;
            hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(ChunkType);
            hashCode = hashCode * -1521134295 + ChunkIndex.GetHashCode();
            hashCode = hashCode * -1521134295 + EntityCount.GetHashCode();
            hashCode = hashCode * -1521134295 + Capacity.GetHashCode();
            hashCode = hashCode * -1521134295 + IsFull.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<List<int>>.Default.GetHashCode(ChunkIDs);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<IPleiadComponent>>.Default.GetHashCode(_componentData);
            return hashCode;
        }
        #endregion
    }
}
