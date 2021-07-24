using System.Threading;
using Pleiad.Tasks.Interfaces;

namespace Pleiad.Tasks
{
    public struct TaskOnHandle<T>
    {
        public TaskOnHandle(IPleiadTaskOn<T> task)
        {
            ActionOn = task.RunOn;
            TokenSource = new CancellationTokenSource();
        }

        public ActionOnDelegate<T> ActionOn { get; }
        public CancellationToken Token { get => TokenSource.Token; }
        public CancellationTokenSource TokenSource { get; }
    }
}



