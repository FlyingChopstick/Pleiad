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
        public static readonly PMesh Quad = new()
        {
            Vertices = new float[]
            { 
                 //X    Y      Z     U   V
                 0.5f,  0.5f, 0.0f, 1f, 1f,
                 0.5f, -0.5f, 0.0f, 1f, 0f,
                -0.5f, -0.5f, 0.0f, 0f, 0f,
                -0.5f,  0.5f, 0.5f, 0f, 1f
            },
            Indices = new uint[]
            {
                0, 1, 3,
                1, 2, 3
            }
        };

        public float[] Vertices { get; init; }
        public uint[] Indices { get; init; }

        public PVertexBufferObject GenerateVertexBuffer(BufferUsageARB usage, GL api)
        {
            return new(BufferTargetARB.ArrayBuffer, Vertices, usage, api);
        }
        public PElementBufferObject GenerateElementBuffer(BufferUsageARB usage, GL api)
        {
            return new(BufferTargetARB.ElementArrayBuffer, Indices, usage, api);
        }
    }
}
