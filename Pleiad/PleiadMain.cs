using System;
using System.Threading.Tasks;
using Pleiad.Entities;
using Pleiad.Input;
using Pleiad.Tasks;
using Pleiad.Worlds;

namespace Pleiad
{
    class PleiadMain : IRegisterInput
    {
        static async Task Main(string[] args)
        {
            World testWorld = new();
            EntityManager em = testWorld.EntityManager;
            var systemsManager = testWorld.SystemsManager;

            TaskManager.EntityManager = em;



            EntityTemplate template1 = new EntityTemplate
                (
                new Type[]
                {
                    typeof(TestComponent),
                    typeof(IntTestComponent)
                },
                new IPleiadComponent[]
                {
                    new TestComponent(),
                    new IntTestComponent() { testValue = 13}
                });

            EntityTemplate template2 = new EntityTemplate
                (
                new Type[]
                {
                    typeof(TestComponent)
                },
                new IPleiadComponent[]
                {
                    new TestComponent() { testValue = "hello" }
                });


            EntityTemplate soundTemplate = new EntityTemplate(
                new Type[]
                {
                    typeof(SoundComponent)
                },
                new IPleiadComponent[]
                {
                    new SoundComponent
                    { files = new string[]
                    {
                        @"Sounds\dice1.wav",
                        @"Sounds\dice2.wav",
                        @"Sounds\dice3.wav",
                        @"Sounds\dice4.wav",
                        @"Sounds\dice5.wav",
                        @"Sounds\dice6.wav"
                    }
                    }
                });

            systemsManager.CreateWindow();
            systemsManager.RunWindow();
            systemsManager.CloseWindow();


            while (testWorld.CanUpdate)
            {

            }

        }

        public void InputRegistration(ref InputListener listener)
        {
            //listener should listen to this key
            listener.ListenTo(Key.Escape);
            //function will handle the event
            listener.KeyRelease += Exit;
        }

        private void Exit(Key key)
        {
            if (key == Key.Escape)
            {
                World.DefaultWorld.StopUpdate();
            }
        }
    }
}
