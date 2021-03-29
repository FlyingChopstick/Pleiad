using System;
using System.Collections.Generic;

namespace PleiadEntities.Tools
{
    [Serializable]
    public struct ChunkSaveObject
    {
        public ChunkSaveObject(EntityChunk chunk)
        {
            ChunkID = chunk.ChunkID;
            Capacity = chunk.Capacity;
            EntityCount = chunk.EntityCount;
            IDs = chunk.ChunkIDs;

            _componentDataObj = chunk.DumpChunkData();

            //_componentDataObj = (List<object>)chunk.GetChunkData();
        }

        public int ChunkID { get; }
        public int Capacity { get; }
        public int EntityCount { get; }
        public List<int> IDs { get; }
        public Type ChunkType
        {
            get
            {
                if (ChunkData.Count == 0)
                {
                    return null;
                }
                return ChunkData[0].GetType();
            }
        }
        public List<IPleiadComponent> ChunkData
        {
            get
            {
                var chunk_d = (List<object>)(_componentDataObj);
                List<IPleiadComponent> components = new List<IPleiadComponent>();

                foreach (var comp in chunk_d)
                {
                    components.Add((IPleiadComponent)comp);
                }
                return components;
            }
        }


        private object _componentDataObj;
    }
}
