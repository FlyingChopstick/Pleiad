using System.Numerics;
using System.Threading.Tasks;
using Pleiad.Entities;
using Pleiad.Input;
using Pleiad.Render.Windows;
using Pleiad.Tasks;
using Pleiad.Worlds;
using Silk.NET.Input;

namespace Pleiad
{
    class PleiadMain : IRegisterInput
    {
        static async Task Main(string[] args)
        {
            //World activeWorld = new();
            World.ActiveWorld = new();
            EntityManager em = World.ActiveWorld.EntityManager;
            var systemsManager = World.ActiveWorld.SystemsManager;
            TaskManager.EntityManager = em;



            //EntityTemplate template1 = new EntityTemplate
            //    (
            //    new Type[]
            //    {
            //        typeof(TestComponent),
            //        typeof(IntTestComponent)
            //    },
            //    new IPleiadComponent[]
            //    {
            //        new TestComponent(),
            //        new IntTestComponent() { testValue = 13}
            //    });

            //EntityTemplate template2 = new EntityTemplate
            //    (
            //    new Type[]
            //    {
            //        typeof(TestComponent)
            //    },
            //    new IPleiadComponent[]
            //    {
            //        new TestComponent() { testValue = "hello" }
            //    });


            //EntityTemplate soundTemplate = new EntityTemplate(
            //    new Type[]
            //    {
            //        typeof(SoundComponent)
            //    },
            //    new IPleiadComponent[]
            //    {
            //        new SoundComponent
            //        { files = new string[]
            //        {
            //            @"Sounds\dice1.wav",
            //            @"Sounds\dice2.wav",
            //            @"Sounds\dice3.wav",
            //            @"Sounds\dice4.wav",
            //            @"Sounds\dice5.wav",
            //            @"Sounds\dice6.wav"
            //        }
            //        }
            //    });



            // added to the SystemsManager ctor


            PWindowOptions options = new()
            {
                Title = "Test Rectangle",
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

            systemsManager.CreateWindow(options, cameraMatrix, projectionMatrix);
            systemsManager.AttachWindow();
            systemsManager.RunWindow();









            //var spritedEntity = em.AddEntity(spriteTemplate);


            //while (World.ActiveWorld.CanUpdate)
            //{

            //}

        }

        public void InputRegistration(ref InputListener listener)
        {
            listener.KeyboardEvents.OnKeyDown += KeyboardEvents_OnKeyDown;
        }

        private void KeyboardEvents_OnKeyDown(IKeyboard keyboard, Key key, int code)
        {
            if (key == Key.Escape)
            {
                World.ActiveWorld.SystemsManager.CloseWindow();
            }
        }
    }
}
