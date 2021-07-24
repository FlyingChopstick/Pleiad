namespace Pleiad.Tasks.Interfaces
{
    public interface IPleiadRenderTask<T>// where T : IRenderable
    {
        void Draw(int index, ref T[] array);
    }
}
