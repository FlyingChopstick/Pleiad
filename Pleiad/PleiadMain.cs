using System.Numerics;
using System.Threading.Tasks;
using Pleiad.Entities;
using Pleiad.Extensions.Files;
using Pleiad.Input;
using Pleiad.Render;
using Pleiad.Render.Windows;
using Pleiad.Systems;
using Pleiad.Tasks;
using Pleiad.Worlds;
using Silk.NET.Input;
using Silk.NET.OpenGL;

namespace Pleiad
{
    class PleiadMain : IRegisterInput
    {
        static async Task Main(string[] args)
        {
            SystemsManager.Init();


            //World.ActiveWorld = new();
            EntityManager em = World.ActiveWorld.EntityManager;
            TaskManager.EntityManager = em;


            PWindowOptions options = new()
            {
                Title = "Test Window",
                Resolution = new()
                {
                    Width = 1280,
                    Height = 720
                },
                VSync = false
            };

            //Setup the camera's location, and relative up and right directions
            Vector3 CameraPosition = new Vector3(0.0f, 0.0f, 3.0f);
            Vector3 CameraTarget = Vector3.Zero;
            Vector3 CameraDirection = Vector3.Normalize(CameraPosition - CameraTarget);
            Vector3 CameraRight = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, CameraDirection));
            Vector3 CameraUp = Vector3.Cross(CameraDirection, CameraRight);

            var cameraMatrix = Matrix4x4.CreateLookAt(CameraPosition, CameraTarget, CameraUp);
            var projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(-1.0f * options.Resolution.Width / options.Resolution.Height,
                1.0f * options.Resolution.Width / options.Resolution.Height,
                -1.0f, 1.0f,
                0.1f, 100.0f);

            PleiadRenderer.VertexShaderSource = new(ShaderType.VertexShader, new FileContract(@"Shaders\shader.vert"));
            PleiadRenderer.FragmentShaderSource = new(ShaderType.FragmentShader, new FileContract(@"Shaders\shader.frag"));
            PleiadRenderer.CreateWindow(options, cameraMatrix, projectionMatrix);
            PleiadRenderer.RunWindow();

        }

        public void InputRegistration(ref InputListener listener)
        {
            listener.KeyboardEvents.OnKeyDown += KeyboardEvents_OnKeyDown;
        }

        private void KeyboardEvents_OnKeyDown(IKeyboard keyboard, Key key, int code)
        {
            if (key == Key.Escape)
            {
                PleiadRenderer.CloseWindow();
            }
        }
    }
}
