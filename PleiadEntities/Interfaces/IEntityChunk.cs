using PleiadMisc;
using System;
using System.Collections.Generic;

namespace PleiadEntities
{
    public interface IEntityChunk
    {
        /// <summary>
        /// Get Chunk ID
        /// </summary>
        int ChunkID { get; }
        /// <summary>
        /// Get Chunk Type
        /// </summary>
        Type ChunkType { get; }
        /// <summary>
        /// Get how much entities does chunk contain
        /// </summary>
        int EntityCount { get; }
        /// <summary>
        /// Maximum amount of entities in chunk
        /// </summary>
        int Capacity { get; }
        /// <summary>
        /// Whether the chunk is full or not
        /// </summary>
        bool IsFull { get; }
        /// <summary>
        /// List of all chunk IDs
        /// </summary>
        IEnumerable<int> ChunkIDs { get; }

        /// <summary>
        /// Check if the chunk contains this entity
        /// </summary>
        /// <param name="entityID">Search for entity ID</param>
        /// <returns><see langword="true"/> if chunk contains this entity</returns>
        bool ContainsID(int entityID);

        /// <summary>
        /// Try to add a new entity to the chunk
        /// </summary>
        /// <param name="entityID">New entity ID</param>
        /// <param name="componentData">Component data of the new entity</param>
        /// <returns>Index of the new entity or -1 if could not add new entity</returns>
        int AddEntity(int entityID, IPleiadComponent componentData);
        /// <summary>
        /// Try to remove entity from the chunk
        /// </summary>
        /// <param name="entityID">Entity to remove</param>
        /// <returns><see langword="true"/> if the entity is deleted successfully, <see langword="false"/> otherwise</returns>
        bool RemoveEntity(int entityID);
        /// <summary>
        /// Returns the component data of the entity
        /// </summary>
        /// <param name="entityID">Entity to retrieve</param>
        /// <returns>IPleiadComponent containing the data</returns>
        Result<IPleiadComponent> GetComponentData(int entityID);
        /// <summary>
        /// Try to set component data of the entity
        /// </summary>
        /// <param name="entityID">Entity ID to set</param>
        /// <param name="componentData">New component data</param>
        /// <returns><see langword="true"/> if the data is updated successfully, <see langword="false"/> otherwise</returns>
        bool SetComponentData(int entityID, IPleiadComponent componentData);

        /// <summary>
        /// Prints all entities in the chunk
        /// </summary>
        public void DEBUG_PrintEntities();
    }
}
