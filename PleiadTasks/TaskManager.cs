using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PleiadEntities;

namespace PleiadTasks
{
    public class TaskManager
    {
        private static readonly Queue<TaskHandle> _handles = new Queue<TaskHandle>();
        private static readonly Queue<CancellationTokenSource> _tokenSources = new Queue<CancellationTokenSource>();
        //private static readonly List<Task> _tasks = new List<Task>();
        private static readonly Queue<Task> _taskQueue = new Queue<Task>();
        private static readonly Queue<Task> _taskOnQueue = new Queue<Task>();

        public static void Enqueue(TaskHandle handle)
        {
            _handles.Enqueue(handle);
            _tokenSources.Enqueue(handle.Source);
            _taskQueue.Enqueue(Task.Run(handle.Action, handle.Token));
        }
        public static void Enqueue(TaskHandle handle, IPleiadComponent[] array, int blockSize = 10)
        {
            int times = array.Length / blockSize;
            if ((times * blockSize) < array.Length) times++;
            int lastIndex = 0;
            for (int i = 0; i < times; i++)
            {
                //set a new task <times> times with a different segment of an array
            }

            _handles.Enqueue(handle);
            _tokenSources.Enqueue(handle.Source);
            _taskOnQueue.Enqueue(Task.Run(handle.Action, handle.Token));
        }


        public static void Complete()
        {
            try
            {
                //int count = _tasks.Count;
                int count = _taskQueue.Count + _taskOnQueue.Count;

                Task.WaitAll(_taskQueue.ToArray());
                Console.WriteLine($"{count} simple tasks completed");
                Task.WaitAll(_taskOnQueue.ToArray());
                Console.WriteLine($"{count} tasks on arrays completed");

                _handles.Clear();
                _tokenSources.Clear();
                //_tasks.Clear();
                _taskQueue.Clear();
            }
            catch (AggregateException ae)
            {
                if (ae.InnerException.GetType() == typeof(TaskCanceledException))
                {
                    Console.WriteLine("A task was canceled.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not complete Tasks! {e.Message}");
                Console.WriteLine(e.StackTrace);
                throw e;
            }


            //try
            //{
            //    int count = _tasks.Count;

            //    Task.WaitAll(_tasks.ToArray());
            //    Console.WriteLine($"{count} tasks completed");

            //    _handles.Clear();
            //    _tokenSources.Clear();
            //    _tasks.Clear();
            //}
            //catch (AggregateException ae)
            //{
            //    if (ae.InnerException.GetType() == typeof(TaskCanceledException))
            //    {
            //        Console.WriteLine("A task was canceled.");
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine($"Could not complete Tasks! {e.Message}");
            //    Console.WriteLine(e.StackTrace);
            //    throw e;
            //}
        }
    }
}
