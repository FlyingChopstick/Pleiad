using System.Collections.Generic;

namespace PleiadEntities
{
    /// <summary>
    /// Lookup for Entity ID - indices of chunks containing this entity
    /// </summary>
    public class EntityChunksLookup : IIndexLookup<int>
    {
        public EntityChunksLookup()
        {
            _lookup = new Dictionary<int, HashSet<int>>();
        }


        /// <summary>
        /// Add Entity ID to lookup
        /// </summary>
        /// <param name="key"></param>
        /// <param name="index"></param>
        public void AddIndex(int entityID, int index)
        {
            if (!_lookup.ContainsKey(entityID))
                _lookup[entityID] = new HashSet<int>();

            _lookup[entityID].Add(index);
        }
        /// <summary>
        /// Get indices of all chunks containing this entity
        /// </summary>
        /// <param name="entityID">Entity ID</param>
        /// <returns>SearchResult with the list of all chunks containing the entity</returns>
        public SearchResult<HashSet<int>> GetIndices(int entityID)
        {
            if (_lookup.ContainsKey(entityID))
                return new SearchResult<HashSet<int>>(_lookup[entityID]);
            else
                return new SearchResult<HashSet<int>>(false);
        }

        public void RemoveIndex(int entityID, int index)
        {
            if (_lookup.ContainsKey(entityID))
                _lookup[entityID].Remove(index);
        }

        public void FlushLookup()
        {
            _lookup.Clear();
        }
        /// <summary>
        /// Remove Entity ID from lookup
        /// </summary>
        /// <param name="entityID"></param>
        public void RemoveKey(int entityID)
        {
            _lookup.Remove(entityID);
        }

        public bool ContainsKey(int entityID) => _lookup.ContainsKey(entityID);

        public int IndexCount(int entityID)
        {
            if (_lookup.ContainsKey(entityID))
                return _lookup[entityID].Count;
            else
                return -1;
        }

        public Dictionary<int, HashSet<int>> Lookup { get => _lookup; }


        private readonly Dictionary<int, HashSet<int>> _lookup;
    }
}
