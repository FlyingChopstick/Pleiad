using Pleiad.Misc.Models;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Interfaces
{
    public interface IBuffer
    {
        BufferTargetARB BufferTarget { get; }
        BufferUsageARB BufferUsage { get; }
        BufferHandle Handle { get; }
        float[] Vertices { get; set; }

        void Bind();
        void SetVerices(float[] vertices);
    }
}
