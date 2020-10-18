using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using PleiadInput;

namespace PleiadSystems
{
    public class SystemsManager
    {
        private readonly Dictionary<Type, SystemPack> _systems;
        private static readonly InputListener _il = new InputListener();
        private readonly Stopwatch _sw;

        private float _lastTime;
        private float _currentTime;

        public float DeltaTime { get; private set; }
        public bool ShouldUpdate { get; set; }

        public bool UseInputTable { get => _il.UseInputTable; set { _il.UseInputTable = value; } }

        /// <summary>
        /// Constructor, tries to load all Systems in the namespace and sets up initial values
        /// </summary>
        public SystemsManager()
        {
            try
            {
                _systems = new Dictionary<Type, SystemPack>();

                LoadSystems();
                RegisterInput();

                //Pause();


                _sw = new Stopwatch();
                _sw.Start();
                _lastTime = 0;
                _currentTime = 0;
                DeltaTime = 0;

                ShouldUpdate = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not start: {e.Message}");
                Console.WriteLine($"{e.StackTrace}");
                throw e;
            }
        }

        /// <summary>
        /// Stops the execution and waits for the key press (default: <see cref="Key.Enter"/>)
        /// </summary>
        /// <param name="key">Key to wait for</param>
        /// <param name="showMessage">Should the message be displayed</param>
        public static void Pause(Key key = Key.Enter, bool showMessage = true)
        {
            if (showMessage)
            {
                Console.WriteLine($"Press {key} to continue");
            }

            //need to temporarily disable InputTable to prevent the key not being found
            bool uit = _il.UseInputTable;
            _il.UseInputTable = false;
            _il.WaitForInput(key);
            _il.UseInputTable = uit;
        }
        /// <summary>
        /// Waits for any of the keys
        /// </summary>
        /// <param name="keys">Keys to wait for</param>
        public static void WaitForInput(Key[] keys)
        {
            _il.WaitForInput(keys);
        }
        /// <summary>
        /// Waits for key press
        /// </summary>
        /// <param name="key">Key to wait for</param>
        public static void WaitForInput(Key key)
        {
            _il.WaitForInput(key);
        }



        private void LoadSystems()
        {
            Console.WriteLine("Loading Systems");
            List<Type> sysInterfaces = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => x.IsInterface && x.Name.Contains("Pleiad") && x.Name.Contains("System")).ToList();

            foreach (var ISys in sysInterfaces)
            {
                try
                {
                    LoadSystem(ISys);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Console.WriteLine("Systems loaded.");
        }
        private void LoadSystem(Type system)
        {
            //Get all classes that implement the selected System interface
            List<Type> systems = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
              .Where(x => system.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
              .ToList();

            if (systems.Count == 0) throw new ArgumentException($"Could not find any Systems of type \"{system}\"");

            List<object> systemObjBuffer = new List<object>();
            List<MethodInfo> methodBuffer = new List<MethodInfo>();
            var sysDict = new Dictionary<object, MethodInfo>();

            foreach (var systemType in systems)
            {
                sysDict[systemType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { })] = systemType.GetMethod("Cycle");
                Console.WriteLine($"  | Loaded {systemType}");
            }

            _systems[system] = new SystemPack(sysDict);
        }


        private void RegisterInput()
        {
            Type ir = typeof(IRegisterInput);
            Console.WriteLine("Input registration");
            //Get all classes that implement the IRegisterInput interface
            List<Type> systems = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
              .Where(x => ir.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
              .ToList();

            //var sysDict = new Dictionary<object, MethodInfo>();
            foreach (var system in systems)
            {
                RegisterInputFor(system);
                Console.WriteLine($"   | Registered {system}");
            }


            Console.WriteLine($"Input registration complete.");
        }
        private void RegisterInputFor(Type system)
        {
            object summoner = system.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            MethodInfo method = system.GetMethod(typeof(IRegisterInput).GetMethods()[0].Name);
            object[] args = new object[] { _il };
            method.Invoke(summoner, args);
        }



        /// <summary>
        /// Function goes over each system in namespace and executes their Cycle()
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            _il.ReadKeys();

            //Update the time
            _currentTime = (float)_sw.Elapsed.TotalMilliseconds;
            DeltaTime = _currentTime - _lastTime;

            foreach (var systemType in _systems.Keys)
            {
                var system = _systems[systemType];


                var objEn = system.Systems.Keys.GetEnumerator();
                var methEn = system.Systems.Values.GetEnumerator();
                //Using an iterator over objects and methods
                while (objEn.MoveNext())
                {
                    methEn.MoveNext();

                    var method = methEn.Current;
                    var summoner = objEn.Current;

                    //Invoke the Cycle()
                    method.Invoke(summoner, new object[] { DeltaTime });
                }

            }

            _lastTime = _currentTime;

            return ShouldUpdate;
        }
    }
}
