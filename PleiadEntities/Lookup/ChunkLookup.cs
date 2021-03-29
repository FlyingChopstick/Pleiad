using PleiadMisc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PleiadEntities
{
    /// <summary>
    /// Lookup for Chunk type - indices of chunks of this type
    /// </summary>
    public class ChunkLookup : IIndexLookup<Type>
    {
        public ChunkLookup()
        {
            _lookup = new Dictionary<Type, HashSet<int>>();
        }
        /// <summary>
        /// Add chunk index to the lookup
        /// </summary>
        /// <param name="chunkType">Type of chunk</param>
        /// <param name="index">Index of chunk</param>
        public void AddIndex(Type chunkType, int index)
        {
            if (!_lookup.ContainsKey(chunkType))
            {
                _lookup[chunkType] = new HashSet<int>();
            }

            _lookup[chunkType].Add(index);
        }
        /// <summary>
        /// Get all chunks of specified type
        /// </summary>
        /// <param name="chunkType">Type of chunk</param>
        /// <returns>SearchResult with the list of all chunks of this type</returns>
        public Result<HashSet<int>> GetIndices(Type chunkType)
        {
            if (_lookup.ContainsKey(chunkType))
                return Result<HashSet<int>>.Found(_lookup[chunkType]);
            else
                return Result<HashSet<int>>.NotFound;
        }

        public void RemoveIndex(Type chunkType, int index)
        {
            if (_lookup.ContainsKey(chunkType))
                _lookup[chunkType].Remove(index);
        }

        public void FlushLookup()
        {
            _lookup.Clear();
        }
        /// <summary>
        /// Remove Type from the lookup
        /// </summary>
        /// <param name="chunkType"></param>
        public void RemoveKey(Type chunkType)
        {
            _lookup.Remove(chunkType);
        }

        public bool ContainsKey(Type chunkType) => _lookup.ContainsKey(chunkType);

        public int IndexCount(Type entityID)
        {
            if (_lookup.ContainsKey(entityID))
                return _lookup[entityID].Count;
            else
                return -1;
        }

        public Dictionary<Type, HashSet<int>> Lookup { get => _lookup; }
        public List<Type> Keys { get => _lookup.Keys.ToList(); }

        private readonly Dictionary<Type, HashSet<int>> _lookup;
    }
}
