using PleiadEntities;
using PleiadExtensions.Files;
using PleiadInput;
using PleiadTasks;
using PleiadWorld;
using System;
using System.Collections.Generic;
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


            EntityTemplate displayTemplate = new EntityTemplate(
                new Type[]
                {
                    typeof(DisplayComponent)
                },
                new IPleiadComponent[]
                {
                    new DisplayComponent()
                    {
                        queue = new List<string>(),
                        output = string.Empty
                    }
                });

            EntityTemplate backgroundTmplt = new EntityTemplate(
                new Type[]
                {
                    typeof(TextureComponent),
                    typeof(IsBackgroundComp)
                },
                new IPleiadComponent[]
                {
                    new TextureComponent()
                    {
                        texture = "|_|_|_|_|_|_|_|"
                    },
                    new IsBackgroundComp()
                    {
                        id = 0
                    }
                });
            EntityTemplate characterTmplt = new EntityTemplate(
                new Type[]
                {
                    typeof(TextureComponent),
                    typeof(IsCharacterComponent)
                },
                new IPleiadComponent[]
                {
                    new TextureComponent()
                    {
                        texture = "   ="
                    },
                    new IsCharacterComponent()
                    {
                        name = "Brave hero"
                    }
                });


            //for (int i = 0; i < 30; i++)
            //{

            //    em.AddEntity(template2);
            //}
            //em.AddEntity(template2);
            //em.AddEntity(template2);
            //em.DEBUG_PrintChunks();
            //em.AddEntity(displayTemplate);
            var be = em.AddEntity(backgroundTmplt);
            var ce = em.AddEntity(characterTmplt);

            //em.RemoveComponent(ref be, typeof(TextureComponent));
            //em.DEBUG_PrintChunks();
            var b = em.GetComponentData<TextureComponent>(be);
            var bID = em.GetComponentData<IsBackgroundComp>(be);
            em.RemoveComponent(be, typeof(TextureComponent));
            //Makes Input listener check only the keys added by ListenTo() 
            //(enabled by default, so you don't have to write this)
            World.DefaultWorld.SystemsManager.UseInputTable = true;

            
            //Same as the cycle below
            //World.DefaultWorld.StartUpdate();
            while (World.DefaultWorld.CanUpdate())
            {
                //TaskHandle simple1 = new TaskHandle(testTask1);
                //TaskHandle simple2 = new TaskHandle(testTask2);

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
