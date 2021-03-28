using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PleiadEntities
{
    [Serializable]
    /// <summary>
    /// Payload is used to set the data for the chunk
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Payload<T> where T : IPleiadComponent
    {
        [JsonConstructor]
        public Payload()
        {

        }
        public Payload(EntityChunk chunk)
        {
            ChunkId = chunk.ChunkID;
            Capacity = chunk.Capacity;
            //ComponentData = chunk.DumpChunkData();
            IDs = chunk.ChunkIDs;
        }

        //public EntityChunk CreateChunk()
        //{
        //    EntityChunk ch = new(ChunkId, typeof(T), Capacity);
        //    ch.SetChunkData(ComponentData);
        //    ch.SetChunkIDs(IDs);
        //    ch.EntityCount = IDs.Where(i => i != -1).Count();
        //    ch.IsFull = ch.EntityCount == ch.Capacity;
        //    return ch;
        //}

        [JsonInclude]
        public int ChunkId { get; set; }
        [JsonInclude]
        public int Capacity { get; set; }
        [JsonInclude]
        public List<T> ComponentData { get; set; }
        [JsonInclude]
        public List<int> IDs { get; set; }

    }
}