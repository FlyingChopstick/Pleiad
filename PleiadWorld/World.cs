using PleiadEntities;
using PleiadSystems;

namespace PleiadWorld
{
    /// <summary>
    /// Contains EntityManager and SystemsManager
    /// </summary>
    public class World
    {
        /// <summary>
        /// Default injection World, static
        /// </summary>
        public static class DefaultWorld
        {
            private static EntityManager _def_em = new EntityManager();
            private static SystemsManager _def_sm = new SystemsManager();
            //private static TaskManager _def_tm = new TaskManager();

            public static float DeltaTime { get => _def_sm.DeltaTime; }

            /// <summary>
            /// World Update
            /// </summary>
            /// <returns><see langword="true"/> if update was successful</returns>
            public static bool CanUpdate()
            {
                return _def_sm.Update();
            }

            /// <summary>
            /// Returns a reference to the Entity manager
            /// </summary>
            public static ref EntityManager EntityManager { get => ref _def_em; }
            public static ref SystemsManager SystemsManager { get => ref _def_sm; }
            //public static ref TaskManager TaskManager { get => ref _def_tm; }
            public static void StartUpdate()
            {
                while (_def_sm.ShouldUpdate)
                    _def_sm.Update();
            }
            public static void StopUpdate()
            {
                _def_sm.ShouldUpdate = false;
            }
        }

        //WIP

        private EntityManager _em;
        private SystemsManager _sm;

        public World()
        {
            _em = new EntityManager();
            _sm = new SystemsManager();
        }

        /// <summary>
        /// Returns a reference to the Entity manager
        /// </summary>
        public ref EntityManager EntityManager { get { return ref _em; } }
    }
}
