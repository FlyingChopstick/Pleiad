using PleiadEntities;
using PleiadInput;
using PleiadWorld;
using System;

namespace Pleiad
{
    class PleiadMain : IRegisterInput
    {
        [STAThread]
        static void Main(string[] args)
        {
            EntityManager em = World.DefaultWorld.EntityManager;

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


            em.AddEntity(soundTemplate);
            //em.DEBUG_PrintChunks();



            //Makes Input listener check only the keys added by ListenTo()
            World.DefaultWorld.SystemsManager.UseInputTable = true;

            //Same as the cycle below
            World.DefaultWorld.StartUpdate();
            //while (World.DefaultWorld.CanUpdate())
            //{
            //}

        }

        public void InputRegistration(ref InputListener listener)
        {
            listener.KeyPress += Exit;
            listener.ListenTo(Key.Escape);
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
