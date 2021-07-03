using Pleiad.Render.Buffers;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Models
{
    public struct PMesh
    {
        public PMesh(float[] vertices, uint[] indices) : this()
        {
            Vertices = vertices;
            Indices = indices;
        }

        public float[] Vertices { get; init; }
        public uint[] Indices { get; init; }

        public VertexBuffer GenerateVertexBuffer(BufferUsageARB usage, GL api)
        {
            return new(BufferTargetARB.ArrayBuffer, usage, api)
            {
                Vertices = Vertices
            };
        }
        public ElementBuffer GenerateElementBuffer(BufferUsageARB usage, GL api)
        {
            return new(BufferTargetARB.ElementArrayBuffer, usage, api)
            {
                Indices = Indices
            };
        }
    }
}
