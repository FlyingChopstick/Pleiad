using System;
using System.Threading;
using PleiadEntities;

namespace PleiadTasks
{
    public struct TaskHandle
    {
        public TaskHandle(IPleiadTask task)
        {
            Action = task.Run;
            ActionOn = null;
            TaskType = typeof(IPleiadTask);
            Source = new CancellationTokenSource();
            Token = Source.Token;
            SearchTemplate = default;
        }
        public TaskHandle(EntityTemplate template, IPleiadTaskOn task)
        {
            Action = null;
            ActionOn = task.RunOn;
            TaskType = typeof(IPleiadTaskOn);
            Source = new CancellationTokenSource();
            Token = Source.Token;
            SearchTemplate = template;
        }

        public Type TaskType { get; }
        public Action Action { get; }
        public Action<int, IPleiadComponent[]> ActionOn { get; }
        public EntityTemplate SearchTemplate { get; }
        public CancellationToken Token { get; }
        public CancellationTokenSource Source { get; }
    }

    public delegate void ActionRef<T, D>(int index, ref D[] array);
}
