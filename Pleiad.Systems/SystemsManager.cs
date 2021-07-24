using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Pleiad.Extensions.Files;
using Pleiad.Input;
using Pleiad.Render;
using Pleiad.Render.Shaders;
using Pleiad.Render.Windows;
using Pleiad.Systems.Interfaces;
using Pleiad.Tasks;
using Silk.NET.OpenGL;

namespace Pleiad.Systems
{
    public class SystemsManager
    {

        public bool ShouldUpdate { get; set; }


        public GL Api => _window.Api;
        private PWindow _window;
        public int WindowHeight => _window.Height;
        public int WindowWidth => _window.Width;
        //public PShader Shader { get => _window.Shader; set => _window.Shader = value; }

        public Matrix4x4 CameraMatrix { get => _window.CameraMatrix; set => _window.CameraMatrix = value; }
        public Matrix4x4 ProjectionMatrix { get => _window.ProjectionMatrix; set => _window.ProjectionMatrix = value; }

        /// <summary>
        /// Constructor, tries to load all Systems in the namespace and sets up initial values
        /// </summary>
        public SystemsManager()
        {
            try
            {
                _systems = new Dictionary<Type, Dictionary<object, MethodInfo>>();
                _renderers = new Dictionary<Type, Dictionary<object, MethodInfo[]>>();

                LoadSystems();
                LoadRenderers();

                ShouldUpdate = true;

            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not start: {e.Message}");
                Console.WriteLine($"{e.StackTrace}");
                throw e;
            }
        }


        public void CreateWindow(PWindowOptions options, Matrix4x4 cameraMatrix, Matrix4x4 projectionMatrix)
        {
            _window = new(options, cameraMatrix, projectionMatrix);
            _window.Updated += Update;
            _window.Load += WindowLoad;
            _window.Render += Render;
        }

        public void AttachWindow()
        {
            _il.AttachToWindow(_window);
            RegisterInput();
        }
        public void RunWindow()
        {
            ShouldUpdate = true;
            _window.Run();
        }
        public void CloseWindow()
        {
            if (_window is not null
                && !_window.IsClosing
                && ShouldUpdate)
            {
                ShouldUpdate = false;
                _window.Close();
            }
        }


        ///// <summary>
        ///// Stops the execution and waits for the key press (default: <see cref="Key.Enter"/>)
        ///// </summary>
        ///// <param name="key">Key to wait for</param>
        ///// <param name="showMessage">Should the message be displayed</param>
        //public static void Pause(Key key = Key.Enter, bool showMessage = true)
        //{
        //    if (showMessage)
        //    {
        //        Console.WriteLine($"Press {key} to continue");
        //    }

        //    //need to temporarily disable InputTable to prevent the key not being found
        //    bool uit = _il.UseInputTable;
        //    _il.UseInputTable = false;
        //    _il.WaitForInput(key);
        //    _il.UseInputTable = uit;
        //}
        ///// <summary>
        ///// Waits for any of the keys
        ///// </summary>
        ///// <param name="keys">Keys to wait for</param>
        //public static void WaitForInput(Key[] keys)
        //{
        //    _il.WaitForInput(keys);
        //}
        ///// <summary>
        ///// Waits for key press
        ///// </summary>
        ///// <param name="key">Key to wait for</param>
        //public static void WaitForInput(Key key)
        //{
        //    _il.WaitForInput(key);
        //}



        private void LoadSystems()
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
        private void LoadSystem(Type system)
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
                sysDict[systemType.GetConstructor(Type.EmptyTypes).Invoke(new object[] { })] = systemType.GetMethod("Cycle");
                Console.WriteLine($"    | Loaded {systemType}");
            }

            _systems[system] = sysDict;
        }

        private void LoadRenderers()
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
        private void LoadRenderer(Type renderType)
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
            Dictionary<object, MethodInfo[]> rendDict = new();

            for (int i = 0; i < renderers.Count; i++)
            {
                MethodInfo[] methods = new MethodInfo[2];
                methods[0] = renderType.GetMethod(nameof(IRenderSystem.Load));
                methods[1] = renderType.GetMethod(nameof(IRenderSystem.Render));
                rendDict[renderers[i].GetConstructor(Type.EmptyTypes).Invoke(new object[] { })] = methods;

                Console.WriteLine($"    | Loaded {renderers[i]}");
            }

            _renderers[renderType] = rendDict;
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

        //public void DrawSprite(PSprite sprite)
        //{
        //    Shader.SetUniform("uModel", sprite.ViewMatrix);
        //    sprite.Draw(Shader);
        //}


        /// <summary>
        /// Function goes over each system in namespace and executes their Cycle()
        /// </summary>
        /// <returns></returns>
        public bool Update(double deltaTime)
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
            return ShouldUpdate;
        }
        private void WindowLoad()
        {
            PleiadRenderer.Shader = CompileShader();
            foreach (var rendererType in _renderers.Keys)
            {
                var objEnum = _renderers[rendererType].Keys.GetEnumerator();
                var methEnum = _renderers[rendererType].Values.GetEnumerator();

                while (objEnum.MoveNext())
                {
                    methEnum.MoveNext();

                    methEnum.Current[0].Invoke(objEnum.Current, null);
                }
            }
        }
        private PShader CompileShader()
        {
            // shaders
            //vertex shader
            FileContract VertexShaderSource = new("Shaders/shader.vert");
            PShaderSource vertexShader = new(ShaderType.VertexShader, VertexShaderSource);
            //fragment shader
            FileContract FragmentShaderSource = new("Shaders/shader.frag");
            PShaderSource fragmentShader = new(ShaderType.FragmentShader, FragmentShaderSource);
            // shader
            return new(Api, vertexShader, fragmentShader);
        }

        private void Render(double obj)
        {
            foreach (var rendererType in _renderers.Keys)
            {
                var objEnum = _renderers[rendererType].Keys.GetEnumerator();
                var methEnum = _renderers[rendererType].Values.GetEnumerator();

                while (objEnum.MoveNext())
                {
                    methEnum.MoveNext();

                    methEnum.Current[1].Invoke(objEnum.Current, new object[] { obj });
                }
            }
        }



        private readonly Dictionary<Type, Dictionary<object, MethodInfo>> _systems;
        private readonly Dictionary<Type, Dictionary<object, MethodInfo[]>> _renderers;
        private static readonly InputListener _il = new();
    }
}
