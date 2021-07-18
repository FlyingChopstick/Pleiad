using Silk.NET.OpenGL;

namespace Pleiad.Render.Models
{
    public struct PMesh<TVertexType, TIndexType>
        where TVertexType : unmanaged
        where TIndexType : unmanaged
    {
        public PMesh(TVertexType[] vertices, TIndexType[] indices)
        {
            Vertices = vertices;
            Indices = indices;
        }

        public static PMesh<float, uint> Quad => new()
        {
            Vertices = new float[]
            { 
                // X      Y      Z      U     V
                -0.5f,  0.5f, 0.0f, 0.0f, 1.0f,
                0.5f,  0.5f, 0.0f, 1.0f, 1.0f,
                0.5f, -0.5f, 0.0f, 1.0f, 0.0f,
                -0.5f, -0.5f, 0.0f, 0.0f, 0.0f,
            },
            Indices = new uint[]
            {
                0, 1, 3,
                1, 2, 3
            }
        };

        public TVertexType[] Vertices { get; init; }
        public TIndexType[] Indices { get; init; }

        public PBufferObject<TVertexType> GenerateVertexBuffer(BufferUsageARB usage, GL api)
        {
            return new(BufferTargetARB.ArrayBuffer, Vertices, usage, api);
        }
        public PBufferObject<TIndexType> GenerateElementBuffer(BufferUsageARB usage, GL api)
        {
            return new(BufferTargetARB.ElementArrayBuffer, Indices, usage, api);
        }
    }
}