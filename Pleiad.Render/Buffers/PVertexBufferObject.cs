using System;
using Pleiad.Render.Handles;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Buffers
{
    public sealed class PVertexBufferObject : IDisposable
    {
        public unsafe PVertexBufferObject(BufferTargetARB bufferTarget, Span<float> vertices, BufferUsageARB bufferUsage, GL api)
        {
            _gl = api;

            BufferTarget = bufferTarget;
            BufferUsage = bufferUsage;

            Handle = new(_gl.GenBuffer());
            Bind();
            fixed (void* v = &vertices[0])
            {
                //set buffer data
                _gl.BufferData(
                    BufferTarget,
                    (nuint)(vertices.Length * sizeof(float)),
                    v,
                    BufferUsage);
            }
        }
        public unsafe void Bind()
        {
            _gl.BindBuffer(BufferTarget, Handle);
        }
        public void Delete()
        {
            _gl.DeleteBuffer(Handle);
        }

        public void Dispose()
        {
            Delete();
        }

        public BufferHandle Handle { get; private set; }
        public BufferTargetARB BufferTarget { get; }
        public BufferUsageARB BufferUsage { get; }

        public static implicit operator BufferHandle(PVertexBufferObject b) => b.Handle;

        public static implicit operator uint(PVertexBufferObject b) => b.Handle;


        private readonly GL _gl;
    }
}
