using PleiadEntities;
using PleiadTasks;
using System;
using System.Threading;

namespace PleiadTasks
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

    public delegate void ActionRef<T, D>(int index, D[] array);
}

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

