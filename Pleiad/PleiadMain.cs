using PleiadEntities;
using PleiadExtensions.Files;
using PleiadInput;
using PleiadMisc;
using PleiadSystems;
using PleiadTasks;
using PleiadWorld;

namespace Pleiad
{
    class PleiadMain : IRegisterInput
    {
        EntityChunk chunk = new(3, typeof(IntTestComponent), 10);
        IPSerialiser<EntityChunk> serialiser = new PSerialiser<EntityChunk>();
        FileContract saveFile = new(@"Models/chunk.save");

        static void Main(string[] args)
        {
            EntityManager em = World.DefaultWorld.EntityManager;
            Tasks.EntityManager = em;
            SystemsManager sm = World.DefaultWorld.SystemsManager;

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
                        World.DefaultWorld.SystemsManager.CloseWindow();
                        break;
                    }
                case Key.S:
                    {
                        chunk.AddEntity(2);
                        chunk.SetComponentData(2, new IntTestComponent() { testValue = 10 });
                        await serialiser.SerialiseAsync(chunk, saveFile);
                        break;
                    }
                case Key.L:
                    {
                        var chunk_L = await serialiser.DeserialiseAsync(saveFile);
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
