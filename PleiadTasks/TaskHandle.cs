using System;
using System.Collections.Generic;
using System.Text;

namespace PleiadTasks
{
    public struct TaskHandle
    {
        public TaskHandle(IPleiadTask task)
        {
            Action = task.Run;
        }
        public Action Action { get; }
    }
}
