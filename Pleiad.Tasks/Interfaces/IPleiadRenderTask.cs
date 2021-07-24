namespace Pleiad.Tasks.Interfaces
{
    public interface IPleiadRenderTask<T>// where T : IRenderable
    {
        void Draw(int i, ref T[] array);
    }
}
