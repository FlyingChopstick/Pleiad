using System.Numerics;
using Pleiad.Entities;
using Pleiad.Systems;

namespace Pleiad.Worlds
{
    /// <summary>
    /// Contains EntityManager and SystemsManager
    /// </summary>
    public class World
    {
        public static World ActiveWorld { get; set; } = null;
        //public static World DefaultWorld { get; } = new();


        private EntityManager _em;
        private SystemsManager _sm;

        public World()
        {
            _em = new EntityManager();
            _sm = new SystemsManager();
        }

        //public PShader Shader { get => _sm.Shader; set => _sm.Shader = value; }
        public Matrix4x4 CameraMatrix { get => _sm.CameraMatrix; set => _sm.CameraMatrix = value; }
        public Matrix4x4 ProjectionMatrix { get => _sm.ProjectionMatrix; set => _sm.ProjectionMatrix= value; }


        //public float DeltaTime { get => _sm.DeltaTime; }

        /// <summary>
        /// Returns a reference to the Entity manager
        /// </summary>
        public ref EntityManager EntityManager { get { return ref _em; } }
        public ref SystemsManager SystemsManager { get => ref _sm; }


        //public bool CanUpdate => _sm.Update();
        //public void StartUpdate()
        //{
        //    while (_sm.ShouldUpdate)
        //    {
        //        _sm.Update();
        //    }
        //}
        public void StopUpdate()
        {
            _sm.ShouldUpdate = false;
        }
    }
}
