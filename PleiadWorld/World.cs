using PleiadEntities;
using PleiadEntities.Tools;
using PleiadExtensions.Files;
using PleiadMisc.Serialisers;
using PleiadSystems;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PleiadWorld
{
    /// <summary>
    /// Contains EntityManager and SystemsManager
    /// </summary>
    public class World
    {
        ///// <summary>
        ///// Default injection World, static
        ///// </summary>
        //public static class DefaultWorld
        //{
        //    private static EntityManager _def_em = new EntityManager();
        //    private static SystemsManager _def_sm = new SystemsManager();
        //    //private static TaskManager _def_tm = new TaskManager();
        //    public static float DeltaTime { get => _def_sm.DeltaTime; }

        //    /// <summary>
        //    /// World Update
        //    /// </summary>
        //    /// <returns><see langword="true"/> if update was successful</returns>
        //    public static bool CanUpdate()
        //    {
        //        return _def_sm.Update();
        //    }


        //    /// <summary>
        //    /// Returns a reference to the Entity manager
        //    /// </summary>
        //    public static ref EntityManager EntityManager { get => ref _def_em; }
        //    public static ref SystemsManager SystemsManager { get => ref _def_sm; }
        //    //public static ref TaskManager TaskManager { get => ref _def_tm; }
        //    public static void StartUpdate()
        //    {
        //        while (_def_sm.ShouldUpdate)
        //            _def_sm.Update();
        //    }
        //    public static void StopUpdate()
        //    {
        //        _def_sm.ShouldUpdate = false;
        //    }

        //    public static ManagerSaveObject SaveManagerState() => new ManagerSaveObject(_def_em);
        //    public static void LoadManagerState(ManagerSaveObject mso)
        //    {
        //        _def_em = new(mso);
        //    }
        //}

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
        public ref EntityManager EntityManager { get => ref _em; }
        public ref SystemsManager SystemsManager { get => ref _sm; }
        public float DeltaTime { get => _sm.DeltaTime; }
        public bool CanUpdate { get => _sm.Update(); }

        public void StartUpdate()
        {
            while (_sm.ShouldUpdate)
                _sm.Update();
        }
        public void StopUpdate()
        {
            _sm.ShouldUpdate = false;
        }

        public ManagerSaveObject SaveManagerState() => new ManagerSaveObject(_em);
        public ref EntityManager LoadManagerState(ManagerSaveObject mso)
        {
            _em = new(mso);
            return ref _em;
        }

        public async Task<ManagerSaveObject> SerialiseManagerAsync(FileContract saveFile)
        {
            ManagerSaveObject mso = SaveManagerState();
            PSerialiser ser = new();

            await ser.SerialiseAsync(mso, saveFile);
            return mso;
        }
        public async Task DeserialiseManagerAsync(FileContract saveFile)
        {
            if (!saveFile.Exists)
            {
                throw new FileNotFoundException("File does not exits");
            }

            PSerialiser ser = new();
            _em = new EntityManager((ManagerSaveObject)await ser.DeserialiseAsync(saveFile));
        }
    }
}
