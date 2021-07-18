using System;
using Pleiad.Render.Handles;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Models
{
    public sealed class PBufferObject<TDataType> : IDisposable
        where TDataType : unmanaged
    {

        public unsafe PBufferObject(BufferTargetARB bufferTarget, Span<TDataType> data, BufferUsageARB bufferUsage, GL api)
        {
            _gl = api;
            BufferTarget = bufferTarget;
            BufferUsage = bufferUsage;

            Handle = new(_gl.GenBuffer());
            Bind();
            fixed (void* v = &data[0])
            {
                //set buffer data
                _gl.BufferData(
                    BufferTarget,
                    (nuint)(data.Length * sizeof(uint)),
                    v,
                    BufferUsage);
            }
        }

        public BufferHandle Handle { get; }
        public BufferTargetARB BufferTarget { get; }
        public BufferUsageARB BufferUsage { get; }

        public unsafe void Bind()
        {
            _gl.BindBuffer(BufferTarget, Handle);
        }
        public void Dispose()
        {
            _gl.DeleteBuffer(Handle);
        }


        public static implicit operator BufferHandle(PBufferObject<TDataType> b) => b.Handle;

        private GL _gl;
    }
}
