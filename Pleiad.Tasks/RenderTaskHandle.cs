using System.Threading;
using Pleiad.Tasks.Interfaces;

namespace Pleiad.Tasks
{
    public struct RenderTaskHandle<T>// where T : IRenderable
    {
        public RenderTaskHandle(IPleiadRenderTask<T> task)
        {
            DrawingAction = task.Draw;
            TokenSource = new();
        }

        public ActionOnDelegate<T> DrawingAction { get; }
        public CancellationToken Token { get => TokenSource.Token; }
        public CancellationTokenSource TokenSource { get; }
    }
}
