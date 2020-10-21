using PleiadEntities;
using PleiadExtensions.Files;
using PleiadInput;
using PleiadTasks;
using PleiadWorld;
using System;
using System.IO;

namespace Pleiad
{
    class PleiadMain : IRegisterInput
    {
        static void Main(string[] args)
        {
            Entities em = World.DefaultWorld.EntityManager;
            Tasks.EntityManager = em;



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



            for (int i = 0; i < 30; i++)
            {

                em.AddEntity(template2);
            }
            em.AddEntity(template2);
            em.AddEntity(template2);
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

            TestTaskOn<TestComponent> testTaskOn = new TestTaskOn<TestComponent>();
            FileContract contract = new FileContract("text.txt");
            File.Delete(contract.FileName);
            TextWriteTask textWriteTask = new TextWriteTask();
            textWriteTask.message = "Writing in main";
            textWriteTask.file = contract;

            TaskHandle textWrite = new TaskHandle(textWriteTask);
            //Same as the cycle below
            //World.DefaultWorld.StartUpdate();
            while (World.DefaultWorld.CanUpdate())
            {
                //TaskHandle simple1 = new TaskHandle(testTask1);
                //TaskHandle simple2 = new TaskHandle(testTask2);

                TaskOnHandle<TestComponent> handleOn = new TaskOnHandle<TestComponent>(testTaskOn);

                //Tasks.SetTask(simple1);
                //Tasks.SetTask(simple2);
                //Tasks.SetTask(handleOn);

                //Tasks.SetTask(textWrite);

                Tasks.CompleteTasks();
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
