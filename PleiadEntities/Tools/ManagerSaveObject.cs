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
            EntityCount = manager.EntityCount;
            ChunkCount = manager.ChunkCount;
            ChunkSize = manager.ChunkSize;
            Chunks = manager.DumpAllChunks();
        }

        public int NextID { get; init; }
        public int EntityCount { get; init; }
        public int ChunkCount { get; init; }
        public int ChunkSize { get; init; }
        public List<List<ChunkSaveObject>> Chunks { get; init; }
    }
}
