using System;
using PleiadEntities;
using PleiadInput;
using PleiadTasks;
using PleiadWorld;

namespace Pleiad
{
    class PleiadMain : IRegisterInput
    {
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
            //(enabled by default, so you don't have to write this)
            World.DefaultWorld.SystemsManager.UseInputTable = true;

            TestTask testTask1 = new TestTask();
            TestTask testTask2 = new TestTask();
            testTask1.value = 0;
            testTask1.num = 31;

            testTask2.value = 0;
            testTask2.num = 2;
            //Same as the cycle below
            //World.DefaultWorld.StartUpdate();
            while (World.DefaultWorld.CanUpdate())
            {
                //Console.Clear();
                testTask1.dTime = World.DefaultWorld.DeltaTime;
                testTask2.dTime = World.DefaultWorld.DeltaTime;
                var handle1 = new TaskHandle(testTask1);
                var handle2 = new TaskHandle(testTask2);
                TaskManager.Enqueue(handle2);
                TaskManager.Enqueue(handle1);


                //TaskManager.Cancel(handle);

                TaskManager.Complete();
            }

        }

        public void InputRegistration(ref InputListener listener)
        {
            //listener should listen to this key
            listener.ListenTo(Key.Escape);
            //function will handle the event
            listener.KeyPress += Exit;
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
