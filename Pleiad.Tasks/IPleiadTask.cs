using Pleiad.Entities;
using System;

namespace Pleiad.Tasks
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
