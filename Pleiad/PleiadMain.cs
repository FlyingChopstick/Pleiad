using System.Numerics;
using System.Threading.Tasks;
using Pleiad.Entities;
using Pleiad.Extensions.Files;
using Pleiad.Input;
using Pleiad.Render;
using Pleiad.Render.Camera;
using Pleiad.Systems;
using Pleiad.Tasks;
using Pleiad.Worlds;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Pleiad
{
    class PleiadMain : IRegisterInput
    {
        static async Task Main(string[] args)
        {
            SystemsManager.Init();
            EntityManager em = World.ActiveWorld.EntityManager;
            TaskManager.EntityManager = em;


            WindowOptions options = WindowOptions.Default;
            options.Title = "Test Window";
            options.Size = new(1280, 720);
            options.FramesPerSecond = 60;
            options.VSync = false;



            Vector3 CameraPosition = new Vector3(0.0f, 0.0f, 3.0f);
            Vector3 CameraTarget = Vector3.Zero;
            PCamera camera = new(CameraPosition, CameraTarget, new(0.0f, 0.0f, -1.0f));
            em.AddCameraEntity(camera);


            var projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(-1.0f * options.Size.X / options.Size.Y,
                1.0f * options.Size.X / options.Size.Y,
                -1.0f, 1.0f,
                0.1f, 100.0f);


            PleiadRenderer.VertexShaderSource = new(ShaderType.VertexShader, new FileContract(@"Shaders\shader.vert"));
            PleiadRenderer.FragmentShaderSource = new(ShaderType.FragmentShader, new FileContract(@"Shaders\shader.frag"));
            PleiadRenderer.CreateWindow(options, camera, projectionMatrix);


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
