using System.Threading;

namespace Pleiad.Tasks
{
    public struct TaskOnHandle<T>
    {
        public TaskOnHandle(IPleiadTaskOn<T> task)
        {
            ActionOn = task.RunOn;
            Source = new CancellationTokenSource();
            Token = Source.Token;
        }

        public ActionOnDelegate ActionOn { get; }
        public CancellationToken Token { get; }
        public CancellationTokenSource Source { get; }


        public delegate void ActionOnDelegate(int index, ref T[] array);
    }


    public delegate void ActionRef<T, D>(int index, D[] array);
}



