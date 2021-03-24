using PleiadTasks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Structure that stores info about task on component chunks
/// </summary>
/// <typeparam name="T">Selected component</typeparam>
public struct TaskOnHandle<T>
{
    public TaskOnHandle(IPleiadTaskOn<T> task)
    {
        ActionOn = task.RunOn;
        Source = new CancellationTokenSource();
        Tasks = new List<Task>();
    }

    /// <summary>
    /// Action to perform on chunks
    /// </summary>
    public ActionOnDelegate ActionOn { get; }
    public CancellationTokenSource Source { get; }
    /// <summary>
    /// List of tasks on chunks
    /// </summary>
    public List<Task> Tasks { get; set; }

    /// <summary>
    /// Delegate to perform action on array
    /// </summary>
    /// <param name="index">Current index</param>
    /// <param name="array">Array of component data</param>
    public delegate void ActionOnDelegate(int index, ref T[] array);


    public static bool operator ==(TaskOnHandle<T> left, TaskOnHandle<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TaskOnHandle<T> left, TaskOnHandle<T> right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        return obj is TaskOnHandle<T> handle &&
               EqualityComparer<ActionOnDelegate>.Default.Equals(ActionOn, handle.ActionOn) &&
               EqualityComparer<CancellationTokenSource>.Default.Equals(Source, handle.Source) &&
               EqualityComparer<List<Task>>.Default.Equals(Tasks, handle.Tasks);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ActionOn, Source, Tasks);
    }
}

