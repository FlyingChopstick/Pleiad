namespace Pleiad.Tasks.Interfaces
{
    public interface IPleiadTaskOn<T>
    {
        void RunOn(int i, ref T[] array);
    }
}
