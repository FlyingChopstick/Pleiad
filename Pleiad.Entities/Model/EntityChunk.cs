using System;
using System.Collections.Generic;
using Pleiad.Entities.Components;

namespace Pleiad.Entities.Model
{
    public class EntityChunk : IEntityChunk
    {
        public int Id { get; }

        private int _count;
        public int Count { get => _count; }

        private readonly int _size;
        public int Size { get => _size; }

        public bool IsOpen { get => _count < _size; }
        public Type ChunkType { get; }

        public const int DefaultChunkSize = 16;


        private List<int> _entities;
        private List<IPleiadComponent> _entitiesData;

        public EntityChunk(int id, Type chunkType, int chunkSize = DefaultChunkSize)
        {
            Id = id;
            ChunkType = chunkType;
            _size = chunkSize;
            _count = 0;

            _entities = new(_size);
            _entitiesData = new(_size);
        }

        public void AddEntity(Entity entity, IPleiadComponent entityData)
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException($"Tried to add a new entity to a closed chunk, Chunk ID{Id}");
            }

            _entities[_count] = entity.Id;
            _entitiesData[_count] = entityData;

            _count++;
        }
        public TComponent GetEntityData<TComponent>(Entity entity) where TComponent : IPleiadComponent
        {
            if (!IsInChunk(entity.Id))
            {
                throw new ArgumentException($"Entity ID{entity} was not in Chunk ID{Id}");
            }

            int index = _entities.IndexOf(entity);
            IPleiadComponent temp = _entitiesData[index];

            return (TComponent)Convert.ChangeType(temp, typeof(TComponent));
        }

        public void RemoveEntity(Entity entity)
        {
            throw new System.NotImplementedException();
        }

        public void SetEntityData(Entity entity)
        {
            throw new System.NotImplementedException();
        }

        private bool IsInChunk(int entityId)
        {
            return _entities.Contains(entityId);
        }
    }
}
