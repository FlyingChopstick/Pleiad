using PleiadEntities;
using PleiadInput;
using PleiadMisc;
using PleiadRender.Abstractions;
using PleiadSystems;
using PleiadTasks;
using PleiadWorld;

namespace Pleiad
{
    class PleiadMain : IRegisterInput
    {
        PShapeData psd = new()
        {
            Vertices = new float[] { 0.4f, 0.5f }
        };
        PSerialiser<PShapeData> serialiser = new();

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
            listener.KeyPress += Listener_KeyPress;
        }
        private void Listener_KeyPress(Key key)
        {
            switch (key)
            {
                case Key.Escape:
                    {
                        World.DefaultWorld.SystemsManager.CloseWindow();
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
