using System;
using System.Collections.Generic;
using Pleiad.Entities.Components;
using Pleiad.Entities.Model;
using Pleiad.Misc.Model;

namespace Pleiad.Entities
{
    public class EntityManager : IEntityManager
    {
        public EntityManager()
        {
            _nextEntityId = 1;
            _nextChunkId = 0;
        }

        public Entity Create(EntityTemplate template)
        {
            Entity newEntity = new(_nextEntityId++);
            var templateData = template.ComponentData;
            for (int i = 0; i < templateData.Length; i++)
            {
                AddToTypeChunk(newEntity, templateData[i]);
            }

            return newEntity;
        }
        public void Remove(Entity entity)
        {
            if (!_entityChunks.ContainsKey(entity))
            {
                throw new ArgumentException("Entity was deleted or not registered");
            }

            foreach ((var type, var index) in _entityChunks[entity])
            {
                _chunks[type][index].RemoveEntity(entity);
            }
        }


        public void AddComponent<TComponent>(Entity target) where TComponent : IPleiadComponent
        {
            throw new System.NotImplementedException();
        }
        public void RemoveComponent<TComponent>(Entity target) where TComponent : IPleiadComponent
        {
            throw new System.NotImplementedException();
        }


        public TComponent GetComponentData<TComponent>(Entity target) where TComponent : IPleiadComponent
        {
            throw new System.NotImplementedException();
        }
        public void SetComponentData<TComponent>(Entity target, TComponent data) where TComponent : IPleiadComponent
        {
            throw new System.NotImplementedException();
        }


        private void AddToTypeChunk<TComponent>(Entity entity, TComponent entityData) where TComponent : IPleiadComponent
        {
            var typeHash = new TypeHash(typeof(TComponent));
            if (_openChunkIndices[typeHash].Count == 0)
            {
                CreateChunk<TComponent>();
            }
            lock (_locker)
            {
                int chunkId = _openChunkIndices[typeHash].Peek();
                _chunks[typeHash][chunkId].AddEntity(entity, entityData);
                CheckChunkIsOpen(typeHash, chunkId);
            }
        }
        private void CheckChunkIsOpen(TypeHash typeHash, int chunkId)
        {
            if (!_chunks[typeHash][chunkId].IsOpen)
            {
                if (_openChunkIndices[typeHash].Peek() != chunkId)
                {
                    throw new InvalidOperationException($"Misaligned current type chunk and chunk in OpenChunkIndices");
                }

                _openChunkIndices[typeHash].Dequeue();
            }
        }
        private int CreateChunk<TComponent>() where TComponent : IPleiadComponent
        {
            var typeHash = new TypeHash(typeof(TComponent));
            if (!_chunks.ContainsKey(typeHash))
            {
                _chunks[typeHash] = new List<IEntityChunk>(DefaultChunkListSize);
            }

            int chunkId = _nextChunkId++;
            _chunks[typeHash].Add(new EntityChunk(chunkId, typeof(TComponent)));
            _openChunkIndices[typeHash].Enqueue(chunkId);
            return chunkId;
        }


        private readonly Dictionary<TypeHash, List<IEntityChunk>> _chunks = new();
        private readonly Dictionary<TypeHash, Queue<int>> _openChunkIndices = new();
        private readonly Dictionary<Entity, List<(TypeHash, int)>> _entityChunks = new();

        private int _nextEntityId;
        private int _nextChunkId;

        private readonly object _locker = new();

        public const int DefaultChunkListSize = 8;
    }
}
