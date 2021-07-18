using System;
using Pleiad.Render.Handles;
using Pleiad.Render.Models;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Arrays
{
    public sealed class PVertexArrayObject<TVertexType, TIndexType> : IDisposable
        where TVertexType : unmanaged
        where TIndexType : unmanaged
    {
        public PVertexArrayObject(GL api, PBufferObject<TVertexType> vbo, PBufferObject<TIndexType> ebo)
        {
            _gl = api;
            Handle = new(_gl.GenVertexArray());
            Bind();
            vbo.Bind();
            ebo.Bind();
        }
        public void Bind()
        {
            _gl.BindVertexArray(Handle);
        }
        public void Unbind()
        {
            _gl.BindVertexArray(0);
        }
        public void Delete()
        {
            _gl.DeleteVertexArray(Handle);
        }
        public unsafe void VertexAttribPointer(uint index, int count, VertexAttribPointerType type, uint vertexSize, int offset)
        {
            _gl.VertexAttribPointer(index, count, type, false, vertexSize * (uint)sizeof(TVertexType), (void*)(offset * sizeof(TVertexType)));
            _gl.EnableVertexAttribArray(index);
        }

        public void Dispose()
        {
            _gl.DeleteVertexArray(Handle);
        }

        public VertexArrayHandle Handle { get; }


        private GL _gl;
    }
}
