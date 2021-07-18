using System;
using Pleiad.Render.Handles;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Buffers
{
    public sealed class PElementBufferObject : IDisposable
    {
        public unsafe PElementBufferObject(BufferTargetARB bufferTarget, Span<uint> indices, BufferUsageARB bufferUsage, GL api)
        {
            _gl = api;
            BufferTarget = bufferTarget;
            BufferUsage = bufferUsage;

            Handle = new(_gl.GenBuffer());
            Bind();
            fixed (void* v = &indices[0])
            {
                //set buffer data
                _gl.BufferData(
                    BufferTarget,
                    (nuint)(indices.Length * sizeof(uint)),
                    v,
                    BufferUsage);
            }
        }
        public unsafe void Bind()
        {
            _gl.BindBuffer(BufferTarget, Handle);
        }

        public void Dispose()
        {
            _gl.DeleteBuffer(Handle);
        }

        public BufferHandle Handle { get; private set; }
        public BufferTargetARB BufferTarget { get; }
        public BufferUsageARB BufferUsage { get; }

        public static implicit operator BufferHandle(PElementBufferObject b) => b.Handle;

        public static implicit operator uint(PElementBufferObject b) => b.Handle;


        private GL _gl;
    }
}
