using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PleiadEntities;
using PleiadEntities.Model;

namespace PleiadTasks
{
    public static class Tasks
    {
        private static readonly List<Task> _taskQueue = new List<Task>();
        private static readonly List<Task> _taskOnQueue = new List<Task>();
        private static Entities _em;
        public static Entities EntityManager { set => _em = value; }

        public static void SetEntityManager(ref Entities entityManager) => _em = entityManager;

        public static void SetTask(TaskHandle handle)
        {
            _taskQueue.Add(Task.Run(handle.Action, handle.Token));
        }
        public static void SetTask<T>(TaskOnHandle<T> handle) where T : IPleiadComponent
        {
            var dataPack = _em.GetTypeData<T>();

            T[] chunkData = dataPack.GetConvertedData();
            int[] chunkSizes = new int[dataPack.ChunkSizes.Keys.Count];
            ChunkIndex[] chunkIndices = dataPack.GetChunkIndices();

            //Run the action for each type chunk
            for (int i = 0; i < chunkIndices.Length; i++)
            {
                ChunkIndex chIndex = new(chunkIndices[i]);
                chunkSizes[i] = dataPack.ChunkSizes[chIndex];
                int currentChSize = chunkSizes[chIndex];
                _taskOnQueue.Add(Task.Run(() =>
                {
                    //List<T> newData = new List<T>();
                    T[] newData = new T[currentChSize];
                    for (int j = 0; j < currentChSize; j++)
                    {
                        handle.ActionOn(j, ref chunkData);
                        newData[j] = chunkData[j];
                    }

                    //var a = newData.ToArray();
                    Payload<T> payload = new Payload<T>(chIndex, newData);
                    _em.SetTypeDataAt(payload);
                }, handle.Token));
            }
        }

        public static void CompleteTasks()
        {
            try
            {
                int count = _taskQueue.Count + _taskOnQueue.Count;

                Task.WaitAll(_taskQueue.ToArray());
                Console.WriteLine($"{_taskQueue.Count} simple tasks completed");
                Task.WaitAll(_taskOnQueue.ToArray());
                Console.WriteLine($"{_taskOnQueue.Count} tasks on arrays completed");

                _taskQueue.Clear();
                _taskOnQueue.Clear();
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
