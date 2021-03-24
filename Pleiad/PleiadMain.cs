using PleiadEntities;
using PleiadInput;
using PleiadSystems;
using PleiadTasks;
using PleiadWorld;

namespace Pleiad
{
    class PleiadMain : IRegisterInput
    {
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
            if (key == Key.Escape)
            {
                World.DefaultWorld.SystemsManager.CloseWindow();
            }
        }
    }
}
