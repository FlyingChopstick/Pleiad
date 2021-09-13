using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Pleiad.Input;
using Pleiad.Render;
using Pleiad.Systems.Interfaces;
using Pleiad.Tasks;

namespace Pleiad.Systems
{
    public static class SystemsManager
    {
        public static bool ShouldUpdate
        {
            get => PleiadRenderer.ShouldUpdate;
            set => PleiadRenderer.ShouldUpdate = value;
        }

        //MOVED INTO PleiadRenderer

        //public static GL Api => _window.Api;
        //private static PWindow _window;
        //public static int WindowHeight => _window.Height;
        //public static int WindowWidth => _window.Width;
        //public PShader Shader { get => _window.Shader; set => _window.Shader = value; }

        //public static Matrix4x4 CameraMatrix { get => _window.CameraMatrix; set => _window.CameraMatrix = value; }
        //public static Matrix4x4 ProjectionMatrix { get => _window.ProjectionMatrix; set => _window.ProjectionMatrix = value; }


        public static void Init()
        {
            try
            {
                _systems = new Dictionary<Type, Dictionary<object, MethodInfo>>();
                _renderers = new Dictionary<Type, Dictionary<object, MethodInfo>>();

                LoadSystems();
                LoadRenderers();

                PleiadRenderer.OnWindowLoad += WindowLoad;
                PleiadRenderer.OnUpdate += Update;
                PleiadRenderer.OnRender += Render;

                ShouldUpdate = true;

            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not start: {e.Message}");
                Console.WriteLine($"{e.StackTrace}");
                throw;
            }
        }
        public static void AttachToRenderer()
        {
            _il.AttachToWindow(PleiadRenderer.Window);
            RegisterInput();
        }


        private static void LoadSystems()
        {
            var systemPostfix = "System";


            Console.WriteLine("Loading Systems");
            List<Type> sysInterfaces = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => x.IsInterface && x.Name.StartsWith("IPleiad") && x.Name.EndsWith(systemPostfix)).ToList();

            foreach (var ISys in sysInterfaces)
            {
                try
                {
                    LoadSystem(ISys);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }

            Console.WriteLine("Systems loaded.");
        }
        private static void LoadSystem(Type system)
        {
            //Get all classes that implement the selected System interface
            List<Type> systems = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
              .Where(x => system.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
              .ToList();

            if (systems.Count == 0)  //throw new ArgumentException($"Could not find any Systems of type \"{system}\"");
            {
                Console.WriteLine($"Could not find any Systems of type \"{system}\"");
            }

            List<object> systemObjBuffer = new List<object>();
            List<MethodInfo> methodBuffer = new List<MethodInfo>();
            var sysDict = new Dictionary<object, MethodInfo>();

            foreach (var systemType in systems)
            {
                sysDict[systemType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { })]
                    = systemType.GetMethod("Cycle");
                Console.WriteLine($"    | Loaded {systemType}");
            }

            _systems[system] = sysDict;
        }

        private static void LoadRenderers()
        {
            Console.WriteLine("Loading Renderers");
            List<Type> sysInterfaces = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => x.IsInterface && x.Name.StartsWith("IRender") && x.Name.EndsWith("System")).ToList();

            for (int i = 0; i < sysInterfaces.Count; i++)
            {
                try
                {
                    LoadRenderer(sysInterfaces[i]);
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    throw;
                }
            }

            Console.WriteLine("Renderers loaded");
        }
        private static void LoadRenderer(Type renderType)
        {
            List<Type> renderers = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => renderType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ToList();

            if (renderers.Count == 0)
            {
                Console.WriteLine($"Could not find any Renderers of type \"{renderType}\"");
            }

            List<object> objBuffer = new(renderers.Count);
            List<MethodInfo> methodBuffer = new(renderers.Count);
            Dictionary<object, MethodInfo> rendDict = new();

            for (int i = 0; i < renderers.Count; i++)
            {
                rendDict[renderers[i].GetConstructor(Type.EmptyTypes).Invoke(new object[] { })]
                    = renderType.GetMethod(nameof(IRenderSystem.Render));
                Console.WriteLine($"    | Loaded {renderers[i]}");
            }

            _renderers[renderType] = rendDict;
        }

        private static void RegisterInput()
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
        private static void RegisterInputFor(Type system)
        {
            object summoner = system.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            MethodInfo method = system.GetMethod(typeof(IRegisterInput).GetMethods()[0].Name);
            object[] args = new object[] { _il };
            method.Invoke(summoner, args);
        }

        //MOVED INTO PleiadRenderer
        // 
        //public void DrawSprite(PSprite sprite)
        //{
        //    Shader.SetUniform("uModel", sprite.ViewMatrix);
        //    sprite.Draw(Shader);
        //}


        /// <summary>
        /// Function goes over each system in namespace and executes their Cycle()
        /// </summary>
        /// <returns></returns>
        private static void WindowLoad()
        {
            AttachToRenderer();

            //PleiadRenderer.Shader = CompileShader();
            //foreach (var rendererType in _renderers.Keys)
            //{
            //    var objEnum = _renderers[rendererType].Keys.GetEnumerator();
            //    var methEnum = _renderers[rendererType].Values.GetEnumerator();

            //    while (objEnum.MoveNext())
            //    {
            //        methEnum.MoveNext();

            //        methEnum.Current.Invoke(objEnum.Current, null);
            //    }
            //}
        }
        private static void Update(double deltaTime)
        {
            foreach (var systemType in _systems.Keys)
            {
                var system = _systems[systemType];


                var objEn = system.Keys.GetEnumerator();
                var methEn = system.Values.GetEnumerator();
                //Using an iterator over objects and methods
                while (objEn.MoveNext())
                {
                    methEn.MoveNext();

                    var method = methEn.Current;
                    var summoner = objEn.Current;

                    //Invoke the Cycle()
                    method.Invoke(summoner, new object[] { deltaTime });
                }

            }

            TaskManager.CompleteTasks();
        }
        private static void Render(double obj)
        {
            foreach (var rendererType in _renderers.Keys)
            {
                var objEnum = _renderers[rendererType].Keys.GetEnumerator();
                var methEnum = _renderers[rendererType].Values.GetEnumerator();

                while (objEnum.MoveNext())
                {
                    methEnum.MoveNext();

                    methEnum.Current.Invoke(objEnum.Current, new object[] { obj });
                }
            }
        }


        private static Dictionary<Type, Dictionary<object, MethodInfo>> _systems;
        private static Dictionary<Type, Dictionary<object, MethodInfo>> _renderers;
        private static readonly InputListener _il = new();
    }
}
