using PleiadTasks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public struct TaskOnHandle<T>
{
    public TaskOnHandle(IPleiadTaskOn<T> task)
    {
        ActionOn = task.RunOn;
        Source = new CancellationTokenSource();
        Tasks = new List<Task>();
    }

    public ActionOnDelegate ActionOn { get; }
    public CancellationTokenSource Source { get; }
    public List<Task> Tasks { get; set; }


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

