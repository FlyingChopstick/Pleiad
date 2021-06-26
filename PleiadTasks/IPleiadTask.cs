using PleiadEntities;
using System;

namespace PleiadTasks
{
    public interface IPleiadTask
    {
        void Run();
    }

    public interface IPleiadTaskOn<T>
    {
        void RunOn(int i, ref T[] array);
    }
}
