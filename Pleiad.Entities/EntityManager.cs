using System.Collections.Concurrent;
using System.Collections.Generic;
using Pleiad.Entities.Components;
using Pleiad.Entities.Model;
using Pleiad.Misc.Model;

namespace Pleiad.Entities
{
    public class EntityManager : IEntityManager
    {
        public Entity Create(EntityTemplate template)
        {
            int chunkIndex;

        }
        public void Remove(Entity entity)
        {
            throw new System.NotImplementedException();
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


        private void AddToChunk(Entity entity, IPleiadComponent entityData)
        {

        }
        private int CreateChunk<TComponent>() where TComponent : IPleiadComponent
        {
            var typeHash = new TypeHash(typeof(TComponent));
            if (!_chunks.ContainsKey(typeHash))
            {
                _chunks[typeHash] = new List<IEntityChunk<IPleiadComponent>>(DefaultChunkListSize);
            }

            _chunks[typeHash].Add(new EntityChunk<TComponent>())
        }


        private ConcurrentDictionary<TypeHash, List<IEntityChunk<IPleiadComponent>>> _chunks = new();
        private ConcurrentDictionary<TypeHash, List<int>> _openChunkIndices = new();

        public const int DefaultChunkListSize = 8;
    }
}
