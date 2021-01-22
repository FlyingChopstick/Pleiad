using PleiadEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PleiadTasks
{
    public static class Tasks
    {
        // to use task handle you need
        //   Action
        //   CancellationToken
        // you need to track running tasks to complete them later
        // you need to queue tasks to chain them after one another
        // 

        //private static readonly Queue<Task> _generalTasks = new Queue<Task>();
        //private static readonly Queue<Dictionary<Task, Task>> _pairedTasks = new Queue<Dictionary<Task, Task>>();
        private static readonly HashSet<Task> _tasks = new HashSet<Task>();
        //private static Entities _em;

        public static Entities EntityManager { get; set; }
        public static int QueuedTasksCount { get => _tasks.Count; }

        //public static void SetTask(TaskHandle handle)
        //{
        //    Task newTask = new Task(handle.Action, handle.Source.Token);
        //    _generalTasks.Enqueue(newTask);
        //    handle.Task = newTask;
        //}
        public static void SetTask(ref TaskHandle handle)
        {
            handle.Task = new Task(handle.Action, handle.Source.Token);
            _tasks.Add(handle.Task);
            handle.Task.Start();
        }
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
                            //handle = prevCopy;
                            //_em.SetChunkData(chunkIndex, )
                        }
                    }
                    //// wait for all of them to complete
                    //Task.WaitAll(taskList.ToArray());
                }
            }

        }

        public static void ChainTasks(ref TaskHandle previous, ref TaskHandle next)
        {
            WaitHandle(ref previous);

            next.Task = new Task(next.Action, next.Source.Token);
            _tasks.Add(next.Task);
            next.Task.Start();
        }
        public static void ChainTasks<T>(ref TaskOnHandle<T> previous, ref TaskHandle next) where T : IPleiadComponent
        {
            //if (previous.Task == default)
            //{
            //    // get all component chunks
            //    var prevCopy = previous;
            //    Type componentType = typeof(T);
            //    var componentChunks = _em.GetAllChunksOfType(componentType);
            //    if (componentChunks.IsFound)
            //    {
            //        List<Task> taskList = new List<Task>(componentChunks.Data.Count);
            //        // set tasks on individual chunks
            //        foreach (var chunkIndex in componentChunks.Data)
            //        {
            //            //get chunk data
            //            var chunkData = _em.GetChunkData(chunkIndex);
            //            if (chunkData.IsFound && chunkData.Data.Count > 0)
            //            {
            //                T[] chunkDataArr = new T[chunkData.Data.Count];
            //                for (int i = 0; i < chunkData.Data.Count; i++)
            //                {
            //                    chunkDataArr[i] = (T)Convert.ChangeType((T)chunkData.Data[i], componentType);
            //                }

            //                //launch & store tasks
            //                taskList.Add(Task.Run(() =>
            //                {
            //                    for (int i = 0; i < chunkData.Data.Count; i++)
            //                    {
            //                        //execute action on chunk data
            //                        prevCopy.ActionOn(i, ref chunkDataArr);
            //                    }
            //                    //update chunk data
            //                    _em.SetChunkData(chunkIndex, chunkDataArr);
            //                }, previous.Source.Token));
            //                previous = prevCopy;
            //                //_em.SetChunkData(chunkIndex, )
            //            }
            //        }
            //        // wait for all of them to complete
            //        Task.WaitAll(taskList.ToArray());
            //    }
            //}

            //for taskOn
            WaitHandle(ref previous);
            // start next task
            SetTask(ref next);
        }
        public static void ChainTasks<T>(ref TaskHandle previous, ref TaskOnHandle<T> next) where T : IPleiadComponent
        {
            WaitHandle(ref previous);
            SetTaskOn(ref next);
        }
        public static void ChainTasks<T>(ref TaskOnHandle<T> previous, ref TaskOnHandle<T> next) where T : IPleiadComponent
        {
            WaitHandle(ref previous);
            SetTaskOn(ref next);
        }

        public static void CompleteTasks()
        {
            Task.WaitAll(_tasks.ToArray());
        }



        private static void WaitHandle(ref TaskHandle handle)
        {
            if (handle != default)
            {
                if (handle.Task == default)
                    SetTask(ref handle);
                handle.Task.Wait();
            }
        }
        private static void WaitHandle<T>(ref TaskOnHandle<T> handle) where T : IPleiadComponent
        {
            if (handle != default)
            {
                if (handle.Tasks.Count == 0)
                    SetTaskOn(ref handle);
                Task.WaitAll(handle.Tasks.ToArray());
            }
        }





        //private static readonly List<Task> _taskQueue = new List<Task>();
        //private static readonly List<Task> _taskOnArrayQueue = new List<Task>();


        //public static int QueuedArrayTasks { get => _taskOnArrayQueue.Count; }



        //public static void SetEntityManager(ref Entities entityManager) => _em = entityManager;

        //public static void SetTask(TaskHandle handle)
        //{
        //    _taskQueue.Add(Task.Run(handle.Action, handle.Token));
        //}



        //public static void SetTask<T>(TaskOnHandle<T> handle) where T : IPleiadComponent
        //{
        //    var dataPack = _em.GetTypeData<T>();

        //    T[] chunkData = dataPack.GetConvertedData();
        //    int[] chunkSizes = new int[dataPack.ChunkSizes.Keys.Count];
        //    int[] chunkIndices = dataPack.GetChunkIndices();

        //    //Run the action for each type chunk
        //    for (int i = 0; i < chunkIndices.Length; i++)
        //    {
        //        int chIndex = chunkIndices[i];
        //        chunkSizes[i] = dataPack.ChunkSizes[chIndex];
        //        int currentChSize = chunkSizes[chIndex];
        //        _taskOnArrayQueue.Add(Task.Run(() =>
        //        {
        //            //List<T> newData = new List<T>();
        //            T[] newData = new T[currentChSize];
        //            for (int j = 0; j < currentChSize; j++)
        //            {
        //                handle.ActionOn(j, ref chunkData);
        //                newData[j] = chunkData[j];
        //            }

        //            //var a = newData.ToArray();
        //            Payload<T> payload = new Payload<T>(chIndex, newData);
        //            _em.SetTypeDataAt(payload);
        //        }, handle.Token));
        //    }
        //}

        //public static void CompleteTasks()
        //{
        //    try
        //    {
        //        int count = _taskQueue.Count + _taskOnArrayQueue.Count;

        //        Task.WaitAll(_taskQueue.ToArray());
        //        //Console.WriteLine($"{_taskQueue.Count} simple tasks completed");
        //        Task.WaitAll(_taskOnArrayQueue.ToArray());
        //        //Console.WriteLine($"{_taskOnArrayQueue.Count} tasks on arrays completed");

        //        _taskQueue.Clear();
        //        _taskOnArrayQueue.Clear();
        //    }
        //    catch (AggregateException ae)
        //    {
        //        if (ae.InnerException.GetType() == typeof(TaskCanceledException))
        //        {
        //            Console.WriteLine("A task was canceled.");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine($"Could not complete Tasks! {e.Message}");
        //        Console.WriteLine(e.StackTrace);
        //        throw e;
        //    }
        //}
    }
}
