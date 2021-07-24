using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pleiad.Entities;
using Pleiad.Entities.Components;

namespace Pleiad.Tasks
{
    public static class TaskManager
    {
        private static readonly List<Task> _taskQueue = new List<Task>();
        private static readonly List<Task> _taskOnQueue = new List<Task>();
        private static EntityManager _em;
        public static EntityManager EntityManager { set => _em = value; }



        public static void SetTask(TaskHandle handle)
        {
            _taskQueue.Add(Task.Run(handle.Action, handle.Token));
        }

        public static void SetTask<T>(TaskOnHandle<T> handle, bool readOnly = false) where T : IPleiadComponent
        {
            Type type = typeof(T);
            var datapack = _em.GetTypeData<T>();
            if (datapack.Data is null)
            {
                return;
            }

            var convertedData = datapack.GetConvertedData();

            if (readOnly)
            {
                for (int i = 0; i < datapack.Length; i++)
                {
                    var index = i;
                    _taskOnQueue.Add(Task.Run(() =>
                    {
                        lock (convertedData[index])
                        {
                            for (int j = 0; j < convertedData[index].Length; j++)
                            {
                                handle.ActionOn(j, ref convertedData[index]);
                            }
                        }
                    }));
                }

                return;
            }


            var updatedDatapack = new DataPack<T>(datapack.Length);
            for (int i = 0; i < datapack.Length; i++)
            {
                _taskOnQueue.Add(Task.Run(() =>
                {
                    int index = i;
                    lock (convertedData[index])
                    {
                        var size = datapack.Data[index].Length;
                        T[] arr = new T[size];
                        for (int j = 0; j < convertedData[index].Length; j++)
                        {
                            handle.ActionOn(j, ref convertedData[index]);
                            arr[j] = convertedData[index][j];
                        }

                        _em.SetDataAt(arr, index);
                    }
                }));
            }
        }
        /// <summary>
        /// Render tasks will be executed on the main thread, without parallelisation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handle"></param>
        public static void SetRenderTask<T>(RenderTaskHandle<T> handle) where T : IPleiadComponent
        {
            var datapack = _em.GetTypeData<T>();
            if (datapack.Data is null)
            {
                return;
            }

            var convertedData = datapack.GetConvertedData().Where(x => x is not null).ToArray();

            for (int i = 0; i < datapack.Length; i++)
            {
                for (int j = 0; j < convertedData[i].Length; j++)
                {
                    handle.DrawingAction(j, ref convertedData[i]);
                }
            }

            return;
        }

        public static void CompleteTasks()
        {
            try
            {
                //int count = _taskQueue.Count + _taskOnQueue.Count;

                Task.WaitAll(_taskQueue.ToArray());
                //Console.WriteLine($"{_taskQueue.Count} simple tasks completed");
                Task.WaitAll(_taskOnQueue.ToArray());
                //Console.WriteLine($"{_taskOnQueue.Count} tasks on arrays completed");

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
