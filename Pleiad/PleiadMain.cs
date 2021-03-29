using PleiadEntities;
using PleiadExtensions.Files;
using PleiadInput;
using PleiadMisc.Serialisers;
using PleiadSystems;
using PleiadTasks;
using PleiadWorld;
using System;

namespace Pleiad
{



    class PleiadMain : IRegisterInput
    {
        static EntityChunk chunk = new(3, typeof(IntTestComponent), 10);
        PSerialiser serialiser = new PSerialiser();
        FileContract saveFile = new(@"Models/chunk.save");

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

            //await em.SaveChunksAsync();
            //await em.LoadChunksAsync();

            //var state = newWorld.SaveManagerState();

            FileContract saveFile = new($"Models/manager.save");

            var state = await newWorld.SerialiseManagerAsync(saveFile);

            em.AddEntity(template2);

            await newWorld.DeserialiseManagerAsync(saveFile);

            em = newWorld.EntityManager;

            //em = newWorld.LoadManagerState(state);

            //var newEm = World.DefaultWorld.EntityManager;

            //string input = Console.ReadLine();
            //Entity startEntity;
            //if (input == "n")
            //{
            //    startEntity = em.AddEntity(template);
            //}
            //else
            //{
            //    if (input == "l")
            //    {
            //        startEntity = new(1);
            //        await em.LoadChunksAsync();
            //        var ch = em.GetAllChunksOfType(typeof(IntTestComponent));
            //    }
            //    else
            //    {
            //        startEntity = new Entity(-1);
            //    }
            //}

            //var gcd = em.GetComponentData<IntTestComponent>(startEntity);






            ////await em.SaveChunksOfTypeAsync(typeof(IntTestComponent));
            //await em.SaveChunksAsync();
            //await em.LoadChunksAsync();

            sm.CreateWindow();
            sm.RunWindow();
            //while (sm.ShouldUpdate)
            //{

            //}

            ////Same as the cycle below
            ////World.DefaultWorld.StartUpdate();
            //while (World.DefaultWorld.CanUpdate())
            //{

            //    World.DefaultWorld.EntityManager.DEBUG_PrintChunks();
            //}
        }


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
