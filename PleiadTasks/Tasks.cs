using PleiadEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PleiadEntities.Entities;

namespace PleiadTasks
{
    public static class Tasks
    {
        private static readonly List<Task> _taskQueue = new List<Task>();
        private static readonly List<Task> _taskOnArrayQueue = new List<Task>();
        private static Entities _em;


        public static int QueuedTasks { get => _taskQueue.Count; }
        public static int QueuedArrayTasks { get => _taskOnArrayQueue.Count; }


        public static Entities EntityManager { set => _em = value; }

        public static void SetEntityManager(ref Entities entityManager) => _em = entityManager;

        public static void SetTask(TaskHandle handle)
        {
            _taskQueue.Add(Task.Run(handle.Action, handle.Token));
        }
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

        public static void CompleteTasks()
        {
            try
            {
                int count = _taskQueue.Count + _taskOnArrayQueue.Count;

                Task.WaitAll(_taskQueue.ToArray());
                //Console.WriteLine($"{_taskQueue.Count} simple tasks completed");
                Task.WaitAll(_taskOnArrayQueue.ToArray());
                //Console.WriteLine($"{_taskOnArrayQueue.Count} tasks on arrays completed");

                _taskQueue.Clear();
                _taskOnArrayQueue.Clear();
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
        }
    }
}
