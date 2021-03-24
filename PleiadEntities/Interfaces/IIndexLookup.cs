using PleiadMisc;
using System.Collections.Generic;

namespace PleiadEntities
{
    public interface IIndexLookup<T>
    {
        /// <summary>
        /// Add an index to the lookup
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="index">Index</param>
        void AddIndex(T key, int index);
        bool ContainsKey(T key);
        /// <summary>
        /// Clear the lookup table
        /// </summary>
        void FlushLookup();
        /// <summary>
        /// Remove index from the lookup
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="index">Index to remove</param>
        void RemoveIndex(T key, int index);
        /// <summary>
        /// Remove the key from the lookup
        /// </summary>
        /// <param name="key">Key to remove</param>
        void RemoveKey(T key);
        /// <summary>
        /// Get all indices from the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Result<HashSet<int>> GetIndices(T key);
        /// <summary>
        /// How many indices does the key contain
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>A number of indices the key contains or -1, if lookup does not contain the key</returns>
        int IndexCount(T key);
        /// <summary>
        /// Lookup Table
        /// </summary>
        Dictionary<T, HashSet<int>> Lookup { get; }
    }
}
