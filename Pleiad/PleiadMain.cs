using PleiadEntities;
using PleiadInput;
using PleiadSystems;
using PleiadWorld;
using System;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.TextFormatting;

namespace Pleiad
{
    class PleiadMain: IRegisterInput
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
            em.DEBUG_PrintChunks();


            while (World.DefaultWorld.CanUpdate())
            {

                Console.WriteLine(World.DefaultWorld.DeltaTime);
            }

        }


        private void Exit()
        {
            World.DefaultWorld.StopUpdate();
        }
        public void Register()
        {
            //InputSystem.Assign(Key.Escape, EventType.Press, Exit);
        }
    }
}
