using Pleiad.Entities;

namespace Pleiad.Worlds
{
    /// <summary>
    /// Contains EntityManager and SystemsManager
    /// </summary>
    public class World
    {
        public static World ActiveWorld { get; set; } = new();

        private EntityManager _em;

        public World()
        {
            _em = new EntityManager();
        }

        /// <summary>
        /// Returns a reference to the Entity manager
        /// </summary>
        public ref EntityManager EntityManager { get { return ref _em; } }
    }
}
