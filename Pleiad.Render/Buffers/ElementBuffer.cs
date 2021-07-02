using Pleiad.Misc.Models;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Buffers
{
    public class ElementBuffer
    {
        public ElementBuffer(BufferTargetARB bufferTarget, BufferUsageARB bufferUsage, GL api)
        {

            _gl = api;
            Handle = new(_gl.GenBuffer());
            BufferTarget = bufferTarget;
            BufferUsage = bufferUsage;
        }
        public void SetVerices(uint[] vertices)
        {
            Indices = vertices;
        }
        public unsafe void Bind()
        {
            _gl.BindBuffer(BufferTarget, Handle);
            fixed (void* v = &Indices[0])
            {
                //set buffer data
                _gl.BufferData(
                    BufferTarget,
                    (nuint)(Indices.Length * sizeof(uint)),
                    v,
                    BufferUsage);
            }
        }

        public uint[] Indices { get; set; }
        public BufferHandle Handle { get; private set; }
        public BufferTargetARB BufferTarget { get; }
        public BufferUsageARB BufferUsage { get; }

        public static implicit operator BufferHandle(ElementBuffer b) => b.Handle;

        public static implicit operator uint(ElementBuffer b) => b.Handle;


        private GL _gl;
    }
}
