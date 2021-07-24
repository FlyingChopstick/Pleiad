using System;
using System.Threading;
using Pleiad.Tasks.Interfaces;

namespace Pleiad.Tasks
{
    public struct TaskHandle
    {
        public TaskHandle(IPleiadTask task)
        {
            Action = task.Run;
            Source = new CancellationTokenSource();
            Token = Source.Token;
        }

        public Action Action { get; }
        public CancellationToken Token { get; }
        public CancellationTokenSource Source { get; }
    }
}



