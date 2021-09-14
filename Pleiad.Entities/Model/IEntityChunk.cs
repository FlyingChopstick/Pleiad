using System;
using Pleiad.Entities.Components;

namespace Pleiad.Entities.Model
{
    public interface IEntityChunk
    {
        int Id { get; }
        int Count { get; }
        int Size { get; }
        bool IsOpen { get; }
        Type ChunkType { get; }

        void AddEntity(Entity entity, IPleiadComponent entityData);
        void RemoveEntity(Entity entity);

        TComponent GetEntityData<TComponent>(Entity entity) where TComponent : IPleiadComponent;
        void SetEntityData(Entity entity, IPleiadComponent entityData);
    }
}
