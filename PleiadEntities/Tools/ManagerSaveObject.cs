using System;
using System.Collections.Generic;

namespace PleiadEntities.Tools
{
    [Serializable]
    public struct ManagerSaveObject
    {
        public ManagerSaveObject(EntityManager manager)
        {
            NextID = manager.NextID;
            //NextChunkIndex = manager.NextChunkIndex;
            EntityCount = manager.EntityCount;
            ChunkCount = manager.ChunkCount;
            ChunkSize = manager.ChunkSize;
            Chunks = manager.DumpAllChunks();
        }

        public int NextID { get; }
        //public int NextChunkIndex { get; }
        public int EntityCount { get; }
        public int ChunkCount { get; }
        public int ChunkSize { get; }
        public List<List<ChunkSaveObject>> Chunks { get; }
    }
}
