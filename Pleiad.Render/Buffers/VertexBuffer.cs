using Pleiad.Misc.Models;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Buffers
{
    public class VertexBuffer
    {
        public VertexBuffer(BufferTargetARB bufferTarget, BufferUsageARB bufferUsage, GL api)
        {

            _gl = api;
            Handle = new(_gl.GenBuffer());
            BufferTarget = bufferTarget;
            BufferUsage = bufferUsage;
        }
        public void SetVerices(float[] vertices)
        {
            Vertices = vertices;
        }
        public unsafe void Bind()
        {
            _gl.BindBuffer(BufferTarget, Handle);
            fixed (void* v = &Vertices[0])
            {
                //set buffer data
                _gl.BufferData(
                    BufferTarget,
                    (nuint)(Vertices.Length * sizeof(uint)),
                    v,
                    BufferUsage);
            }
        }

        public float[] Vertices { get; set; }
        public BufferHandle Handle { get; private set; }
        public BufferTargetARB BufferTarget { get; }
        public BufferUsageARB BufferUsage { get; }

        public static implicit operator BufferHandle(VertexBuffer b) => b.Handle;

        public static implicit operator uint(VertexBuffer b) => b.Handle;


        private GL _gl;
    }
}
