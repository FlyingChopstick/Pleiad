namespace PleiadTasks
{
    public interface IPleiadTaskOn<T>
    {
        void RunOn(int i, ref T[] array);
    }
}
