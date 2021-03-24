using System;
using System.Threading;
using System.Threading.Tasks;

namespace PleiadTasks
{
    /// <summary>
    /// Structure that stores task info
    /// </summary>
    public struct TaskHandle
    {
        public TaskHandle(IPleiadTask task)
        {
            Action = task.Run;
            Source = new CancellationTokenSource();
            Task = default;
        }

        /// <summary>
        /// What task should do
        /// </summary>
        public Action Action { get; }
        public CancellationTokenSource Source { get; }
        /// <summary>
        /// Task object to track
        /// </summary>
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
}

