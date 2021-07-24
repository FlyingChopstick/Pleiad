using System;
using System.Numerics;
using Pleiad.Render.Camera;
using Pleiad.Render.Models;
using Pleiad.Render.Shaders;
using Pleiad.Render.Windows;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Pleiad.Render
{
    public static class PleiadRenderer
    {
        public static event PWindow.WindowLoadDelegate OnWindowLoad;
        public static event PWindow.UpdatedDelegate OnUpdate;
        public static event PWindow.RenderDelegate OnRender;
        public static event PWindow.ClosedDelegate OnClose;


        public static GL Api { get => _window.Api; }

        public static PShaderSource VertexShaderSource { get; set; }
        public static PShaderSource FragmentShaderSource { get; set; }

        public static PShader Shader { get => _shader; }
        private static PShader _shader;

        public static PCamera Camera { get; set; }
        //public static Matrix4x4 CameraMatrix { get; set; }
        public static Matrix4x4 ProjectionMatrix { get; set; }

        public static bool ShouldUpdate { get; set; }

        public static PWindow Window { get => _window; }
        private static PWindow _window;
        public static int WindowHeight { get => _window.Height; }
        public static int WindowWidth { get => _window.Width; }


        public static void CreateWindow(WindowOptions options, PCamera camera, Matrix4x4 projectionMatrix)
        {
            _window = new(options);
            _window.Load += Load; ;
            _window.Updated += Update;
            _window.Render += Render;
            _window.Closed += Closing;

            Camera = camera;
            ProjectionMatrix = projectionMatrix;
        }
        public static void RunWindow()
        {
            ShouldUpdate = true;
            _window.Run();
        }
        public static void CloseWindow()
        {
            if (_window is not null
                && !_window.IsClosing
                && ShouldUpdate)
            {
                ShouldUpdate = false;
                _window.Close();
            }
        }


        public static void DrawSprite(PSprite sprite)
        {
            _shader.SetUniform("uModel", sprite.ViewMatrix);
            sprite.Draw(_shader);
        }


        private static void CompileShader()
        {
            if (VertexShaderSource is null
                || FragmentShaderSource is null)
            {
                ThrowError(ErrorType.NoShaderSources);
            }

            _shader = new(Api, VertexShaderSource, FragmentShaderSource);
        }


        // event handlers/forwarders
        private static void Load()
        {
            CompileShader();

            OnWindowLoad?.Invoke();
        }
        private static void Update(double deltaTime)
        {
            OnUpdate?.Invoke(deltaTime);
        }
        private static void Render(double obj)
        {
            OnRender?.Invoke(obj);

            _shader.SetUniform("uView", Camera);
            _shader.SetUniform("uProjection", PleiadRenderer.ProjectionMatrix);
        }
        private static void Closing()
        {
            OnClose?.Invoke();
        }



        private enum ErrorType
        {
            NoShaderSources = 0,
        }
        private static void ThrowError(ErrorType t)
        {
            string msg = t switch
            {
                ErrorType.NoShaderSources => "Shader sources were not set up. Shader sources should be added to the Renderer before the window Load",
                _ => string.Empty
            };

            throw new ArgumentException(msg);
        }
    }
}
