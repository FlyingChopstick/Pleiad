using System;
using System.Threading;
using System.Threading.Tasks;

namespace PleiadTasks
{
    public struct TaskHandle
    {
        public TaskHandle(IPleiadTask task)
        {
            Action = task.Run;
            Source = new CancellationTokenSource();
            Task = default;
        }

        public Action Action { get; }
        public CancellationTokenSource Source { get; }
        public Task Task { get; set; }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(TaskHandle left, TaskHandle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TaskHandle left, TaskHandle right)
        {
            return !(left == right);
        }
    }

    public delegate void ActionRef<T, D>(int index, D[] array);
}

