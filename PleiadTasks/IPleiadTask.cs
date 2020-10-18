using PleiadEntities;

namespace PleiadTasks
{
    public interface IPleiadTask
    {
        void Run();
    }

    public interface IPleiadTaskOn
    {
        void RunOn<T>(int i, T[] array);
    }
}
