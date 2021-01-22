using PleiadEntities;
using PleiadInput;
using PleiadTasks;
using PleiadWorld;
using System;
using System.Collections.Generic;

namespace Pleiad
{
    class PleiadMain : IRegisterInput
    {
        static void Main(string[] args)
        {
            Entities em = World.DefaultWorld.EntityManager;
            Tasks.EntityManager = em;



            EntityTemplate tmpl_string_int = new EntityTemplate
                (
                new Type[]
                {
                    typeof(StringTestComponent),
                    typeof(IntTestComponent)
                },
                new IPleiadComponent[]
                {
                    new StringTestComponent(),
                    new IntTestComponent() { testValue = 13}
                });

            EntityTemplate tmpl_string = new EntityTemplate
                (
                new Type[]
                {
                    typeof(StringTestComponent)
                },
                new IPleiadComponent[]
                {
                    new StringTestComponent() { text = "hello" }
                });


            EntityTemplate tmpl_sound = new EntityTemplate(
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


            EntityTemplate tmpl_display = new EntityTemplate(
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

            EntityTemplate tmpl_background = new EntityTemplate(
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
            EntityTemplate tmpl_character = new EntityTemplate(
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
            var string1 = em.AddEntity(tmpl_string);
            var string2 = em.AddEntity(tmpl_string);
            var string3 = em.AddEntity(tmpl_string);


            var consoleWrite1 = new ReadAllStrings();
            var consoleWrite2 = new ReadAllStrings();
            var consoleWrite3 = new ReadAllStrings();

            //Same as the cycle below
            //World.DefaultWorld.StartUpdate();
            while (World.DefaultWorld.CanUpdate())
            {
                var sh1 = new TaskOnHandle<StringTestComponent>(consoleWrite1);
                var sh2 = new TaskOnHandle<StringTestComponent>(consoleWrite2);
                var sh3 = new TaskOnHandle<StringTestComponent>(consoleWrite3);

                Tasks.SetTaskOn(ref sh1);
                Tasks.ChainTasks(ref sh1, ref sh2);
                Tasks.ChainTasks(ref sh2, ref sh3);

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
