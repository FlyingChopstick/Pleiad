using PleiadEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace PleiadSystems
{
    public class SystemsManager
    {
        private Dictionary<Type, SystemPack> _systems;

        private float _currentTime;
        private float _lastTime;
        private Stopwatch _sw;
        private EntityManager _em;

        public float DeltaTime { get; private set; }
        public bool ShouldUpdate { get; set; }

        /// <summary>
        /// Constructor, tries to load all Systems in the namespace and sets up initial values
        /// </summary>
        public SystemsManager(ref EntityManager entityManager)
        {
            try
            {
                _em = entityManager;
                _systems = new Dictionary<Type, SystemPack>();



                Console.WriteLine("Loading Systems...");
                List<Type> sysInterfaces = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                    .Where(x => x.IsInterface && x.Name.Contains("Pleiad") && x.Name.Contains("System")).ToList();

                foreach (var ISys in sysInterfaces)
                {
                    try
                    {
                        if (ISys.Name.Contains("For"))
                            LoadSystem(SystemIs.For, ISys);
                        else
                            LoadSystem(SystemIs.Simple, ISys);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                Console.WriteLine("All Systems loaded successfully.");

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
        /// Loads all specified Systems
        /// </summary>
        /// <param name="system">System type</param>
        private void LoadSystem(SystemIs category, Type system)
        {
            Console.WriteLine($"-- Loading {system}");
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
                Console.WriteLine($"--- Loading {systemType}");
                //Store the MethodInfo
                //methodBuffer.Add(systemType.GetMethod("Cycle"));

                //Construct and store the System object
                //systemObjBuffer.Add(systemType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { }));

                sysDict[systemType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { })] = systemType.GetMethod("Cycle");
            }

            //Bind method to an object
            //for (int i = 0; i < methodBuffer.Count; i++)
            //{
            //    sysDict[systemObjBuffer[i]] = methodBuffer[i];
            //}

            _systems[system] = new SystemPack(category, sysDict);
            Console.WriteLine($"-- Loaded.");
        }

        /// <summary>
        /// Function goes over each system in namespace and executes their Cycle()
        /// </summary>
        /// <returns></returns>
        public bool Update()
        {
            foreach (var systemType in _systems.Keys)
            {
                var system = _systems[systemType];

                //Update the time
                _currentTime = (float)_sw.Elapsed.TotalMilliseconds;
                DeltaTime = _currentTime - _lastTime;

                var objEn = system.Systems.Keys.GetEnumerator();
                var methEn = system.Systems.Values.GetEnumerator();
                switch (system.Type)
                {
                    case SystemIs.Simple:
                        {
                            //Using an iterator over objects and methods
                            while (objEn.MoveNext())// && _AS_methods.MoveNext())
                            {
                                methEn.MoveNext();

                                var method = methEn.Current;
                                var summoner = objEn.Current;

                                //Invoke the Cycle()
                                method.Invoke(summoner, new object[] { DeltaTime });
                            }

                            break;
                        }
                    case SystemIs.For:
                        {

                            //Using an iterator over objects and methods
                            while (objEn.MoveNext())// && _AS_methods.MoveNext())
                            {
                                methEn.MoveNext();


                                var method = methEn.Current;
                                var summoner = objEn.Current;

                                //Invoke the Cycle()
                                method.Invoke(summoner, new object[] { DeltaTime, system.Query });
                            }

                            break;
                        }
                }


                //Reset iterators to the beginning

                _lastTime = _currentTime;
            }


            return ShouldUpdate;
        }

    }
}
