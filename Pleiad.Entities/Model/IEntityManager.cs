using Pleiad.Entities.Components;

namespace Pleiad.Entities.Model
{
    public interface IEntityManager
    {
        Entity Create(EntityTemplate template);
        void Remove(Entity entity);

        void AddComponent<TComponent>(Entity target) where TComponent : IPleiadComponent;
        void RemoveComponent<TComponent>(Entity target) where TComponent : IPleiadComponent;

        TComponent GetComponentData<TComponent>(Entity target) where TComponent : IPleiadComponent;
        void SetComponentData<TComponent>(Entity target, TComponent data) where TComponent : IPleiadComponent;
    }
}
