using System.Collections.Generic;
using System.Threading;

namespace PleiadTasks
{
    public class TaskManager
    {
        private readonly List<TaskHandle> _handles;
        private readonly List<Thread> _threads;

        public TaskManager()
        {
            _handles = new List<TaskHandle>();
            _threads = new List<Thread>();
        }

        public void Schedule(TaskHandle handle)
        {
            if (!_handles.Contains(handle))
            {
                Thread newThread = new Thread(new ThreadStart(handle.Action));
                newThread.Start();
            }
        }

        public void Complete()
        {
            foreach (var thread in _threads)
            {
                thread.Join();
            }
            _handles.Clear();
            _threads.Clear();
        }
    }
}
