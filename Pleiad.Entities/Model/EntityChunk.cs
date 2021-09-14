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


        private readonly List<Entity> _entities;
        private readonly List<IPleiadComponent> _entitiesData;
        private readonly Queue<int> _openIndices;

        public EntityChunk(int id, Type chunkType, int chunkSize = DefaultChunkSize)
        {
            Id = id;
            ChunkType = chunkType;
            _size = chunkSize;
            _count = 0;

            _entities = new(_size);
            _entitiesData = new(_size);
            _openIndices = new Queue<int>(_size);

            for (int i = 0; i < _size; i++)
            {
                _entities.Add(default);
                _entitiesData.Add(default);
                _openIndices.Enqueue(i);
            }
        }

        public void AddEntity(Entity entity, IPleiadComponent entityData)
        {
            if (!IsOpen)
            {
                throw new InvalidOperationException(
                    $"Tried to add a Entity ID{entity} to a closed chunk, Chunk ID{Id}");
            }
            if (ChunkType != entityData.GetType())
            {
                throw new ArgumentException(
                    $"Tried to add data with type {entityData.GetType()} to the chunk of {ChunkType}");
            }

            int index = _openIndices.Dequeue();
            _entities[index] = entity;
            _entitiesData[index] = entityData;

            _count++;
        }
        public TComponent GetEntityData<TComponent>(Entity entity) where TComponent : IPleiadComponent
        {
            CheckIsInChunk(entity);

            int index = _entities.IndexOf(entity);
            IPleiadComponent temp = _entitiesData[index];

            return (TComponent)Convert.ChangeType(temp, typeof(TComponent));
        }


        public void RemoveEntity(Entity entity)
        {
            CheckIsInChunk(entity);

            int index = _entities.IndexOf(entity);
            _entities[index] = default;
            _entitiesData[index] = default;

            _openIndices.Enqueue(index);
            _count--;
        }

        public void SetEntityData(Entity entity, IPleiadComponent entityData)
        {
            CheckDataType(entityData);

            int index = _entities.IndexOf(entity);
            _entitiesData[index] = entityData;
        }

        private void CheckDataType(IPleiadComponent entityData)
        {
            if (entityData.GetType() != ChunkType)
            {
                throw new ArgumentException($"Tried to add data type {entityData.GetType()} to the chunk of type {ChunkType}");
            }
        }
        private void CheckIsInChunk(Entity entity)
        {
            if (!_entities.Contains(entity))
            {
                throw new ArgumentException($"Entity ID{entity} was not in Chunk ID{Id}");
            }
        }
    }
}
