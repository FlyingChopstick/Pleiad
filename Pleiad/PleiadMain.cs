using System.Threading.Tasks;
using Pleiad.Entities;
using Pleiad.Input;
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
            systemsManager.CreateWindow();
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
