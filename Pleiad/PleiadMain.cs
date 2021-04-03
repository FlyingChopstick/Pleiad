using PleiadEntities;
using PleiadInput;
using PleiadMisc.Files;
using PleiadSystems;
using PleiadTasks;
using PleiadWorld;
using System;

namespace Pleiad
{
    //Документации пока нет, пиши в лс

    //World содержит в себе EntityManager и SystemsManager
    //SystemsManager содержит окно (PWindow)
    //Система чтения клавиатуры находится в Pleiad/Test.cs


    class PleiadMain// : IRegisterInput
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            World newWorld = new();

            EntityManager em = newWorld.EntityManager;
            Tasks.EntityManager = em;
            SystemsManager sm = newWorld.SystemsManager;

            var template = new EntityTemplate(
                    new Type[] { typeof(IntTestComponent) },
                    new IPleiadComponent[]
                    {
                        new IntTestComponent()
                        {
                            testValue = 26
                        }
                    }
                    );
            var template2 = new EntityTemplate(
                    new Type[] { typeof(StringTestComponent) },
                    new IPleiadComponent[]
                    {
                        new StringTestComponent()
                        {
                            text = "text"
                        }
                    }
                    );
            em.AddEntity(template);


            //serialisation
            //save file
            FileContract saveFile = new($"Models/manager.save");
            //saved manager state (debug)
            var state = await newWorld.SerialiseManagerAsync(saveFile);
            //add entity AFTER save
            em.AddEntity(template2);

            //deserialisation
            await newWorld.DeserialiseManagerAsync(saveFile);
            //update manager handle
            //it shouldn't contain the added entity
            em = newWorld.EntityManager;


            Console.Clear();

            //create opengl window
            sm.CreateWindow();
            sm.RunWindow();

            //default update cycle (deprecated)
            //while (sm.ShouldUpdate)
            //{

            //}

            ////Same as the cycle (deprecated)
            ////World.DefaultWorld.StartUpdate();
        }


        //Использовалось для проверки сериализации и закрытия окна, сейчас выключено
        public void InputRegistration(ref InputListener listener)
        {
            listener.ListenTo(Key.Escape);
            listener.ListenTo(Key.S);
            listener.ListenTo(Key.L);
            listener.KeyPress += Listener_KeyPress;
        }
        private async void Listener_KeyPress(Key key)
        {
            switch (key)
            {
                case Key.Escape:
                    {
                        //World.DefaultWorld.SystemsManager.CloseWindow();
                        break;
                    }
                case Key.S:
                    {
                        //Payload<IntTestComponent> payload = new(chunk);

                        //await payloadManager.SaveAsync(payload, saveFile);
                        //chunk.AddEntity(2);
                        //chunk.SetComponentData(2, new IntTestComponent() { testValue = 10 });
                        //await serialiser.SerialiseAsync(chunk, saveFile);

                        break;
                    }
                case Key.L:
                    {
                        //var data = await payloadManager.LoadAsync(saveFile);
                        //EntityChunk newChunk = data.CreateChunk();
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
