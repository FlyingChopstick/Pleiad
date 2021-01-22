using PleiadEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PleiadTasks
{
    /// <summary>
    /// Manages creating, starting and completing threaded tasks
    /// </summary>
    public static class Tasks
    {

        /// <summary>
        /// Entity manager that is used
        /// </summary>
        public static Entities EntityManager { get; set; }
        /// <summary>
        /// Amount of queued tasks;
        /// </summary>
        public static int QueuedTasksCount { get => _tasks.Count; }

        /// <summary>
        /// Sets a new task
        /// </summary>
        /// <param name="handle">Handle for task</param>
        public static void SetTask(ref TaskHandle handle)
        {
            handle.Task = new Task(handle.Action, handle.Source.Token);
            _tasks.Add(handle.Task);
            handle.Task.Start();
        }
        /// <summary>
        /// Sets a new task on component chunks
        /// </summary>
        /// <param name="handle">Handle for task</param>
        public static void SetTaskOn<T>(ref TaskOnHandle<T> handle) where T : IPleiadComponent
        {
            if (handle.Tasks.Count == 0)
            {
                // get all component chunks

                //local copy of the handle
                var handleCopy = handle;
                Type componentType = typeof(T);
                var componentChunks = EntityManager.GetAllChunksOfType(componentType);
                if (componentChunks.IsFound)
                {
                    List<Task> taskList = new List<Task>(componentChunks.Data.Count);
                    // set tasks on individual chunks
                    foreach (var chunkIndex in componentChunks.Data)
                    {
                        //get chunk data
                        var chunkData = EntityManager.GetChunkData(chunkIndex);
                        if (chunkData.IsFound && chunkData.Data.Count > 0)
                        {
                            T[] chunkDataArr = new T[chunkData.Data.Count];
                            List<int> entityIndices = new List<int>();
                            for (int i = 0; i < chunkData.Data.Count; i++)
                            {
                                if (chunkData.Data[i] != default)
                                {
                                    chunkDataArr[i] = (T)Convert.ChangeType((T)chunkData.Data[i], componentType);
                                    entityIndices.Add(i);
                                }
                            }

                            //launch & store tasks
                            var newTask = Task.Run(() =>
                            {
                                for (int i = 0; i < entityIndices.Count; i++)
                                {
                                    //execute action on chunk data
                                    handleCopy.ActionOn(entityIndices[i], ref chunkDataArr);
                                }
                                //update chunk data
                                EntityManager.SetChunkData(chunkIndex, chunkDataArr);
                            }, handle.Source.Token);

                            taskList.Add(newTask);
                            handle.Tasks.Add(newTask);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Wait for previous task to finish and start next
        /// </summary>
        /// <param name="previous">This task must complete</param>
        /// <param name="next">Next task to run</param>
        public static void ChainTasks(ref TaskHandle previous, ref TaskHandle next)
        {
            WaitHandle(ref previous);

            next.Task = new Task(next.Action, next.Source.Token);
            _tasks.Add(next.Task);
            next.Task.Start();
        }
        /// <summary>
        /// Wait for previous task to finish and start next
        /// </summary>
        /// <param name="previous">This task must complete</param>
        /// <param name="next">Next task to run</param>
        public static void ChainTasks<T>(ref TaskOnHandle<T> previous, ref TaskHandle next) where T : IPleiadComponent
        {
            WaitHandle(ref previous);
            SetTask(ref next);
        }
        /// <summary>
        /// Wait for previous task to finish and start next
        /// </summary>
        /// <param name="previous">This task must complete</param>
        /// <param name="next">Next task to run</param>
        public static void ChainTasks<T>(ref TaskHandle previous, ref TaskOnHandle<T> next) where T : IPleiadComponent
        {
            WaitHandle(ref previous);
            SetTaskOn(ref next);
        }
        /// <summary>
        /// Wait for previous task to finish and start next
        /// </summary>
        /// <param name="previous">This task must complete</param>
        /// <param name="next">Next task to run</param>
        public static void ChainTasks<T>(ref TaskOnHandle<T> previous, ref TaskOnHandle<T> next) where T : IPleiadComponent
        {
            WaitHandle(ref previous);
            SetTaskOn(ref next);
        }

        /// <summary>
        /// Wait for all tasks to complete
        /// </summary>
        public static void CompleteTasks()
        {
            Task.WaitAll(_tasks.ToArray());
            _tasks.Clear();
        }


        /// <summary>
        /// Wait for that task to complete
        /// </summary>
        /// <param name="handle">Handle for task</param>
        private static void WaitHandle(ref TaskHandle handle)
        {
            if (handle != default)
            {
                if (handle.Task == default)
                    SetTask(ref handle);
                handle.Task.Wait();
            }
        }
        /// <summary>
        /// Wait for that task to complete
        /// </summary>
        /// <param name="handle">Handle for task</param>
        private static void WaitHandle<T>(ref TaskOnHandle<T> handle) where T : IPleiadComponent
        {
            if (handle != default)
            {
                if (handle.Tasks.Count == 0)
                    SetTaskOn(ref handle);
                Task.WaitAll(handle.Tasks.ToArray());
            }
        }

        /// <summary>
        /// List of all set tasks
        /// </summary>
        private static readonly HashSet<Task> _tasks = new HashSet<Task>();
    }
}
