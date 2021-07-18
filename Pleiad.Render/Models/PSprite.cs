using System;
using Pleiad.Render.Arrays;
using Pleiad.Render.Buffers;
using Pleiad.Render.Shaders;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Models
{
    public sealed class PSprite : IDisposable
    {
        private readonly GL _gl;
        private readonly PTexture _texture;
        private readonly PShader _shader;
        private readonly PMesh _quad = PMesh.Quad;
        private PVertexBufferObject _vbo;
        private PElementBufferObject _ebo;
        private PVertexArrayObject<float, uint> _vao;

        public PSprite(GL api, PTexture texture, PShader shader)
        {
            _gl = api;

            _texture = texture;
            _shader = shader;
        }
        public unsafe void Load()
        {
            _ebo = _quad.GenerateElementBuffer(BufferUsageARB.StaticDraw, _gl);
            _vbo = _quad.GenerateVertexBuffer(BufferUsageARB.StaticDraw, _gl);

            _vao = new(_gl, _vbo, _ebo);
            _vao.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, 5, 0);
            _vao.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, 5, 3);
        }
        public unsafe void Draw()
        {
            _vao.Bind();
            _shader.Use();
            _texture.Bind();
            _shader.SetUniform("uTexture0", 0);

            _gl.DrawElements(
                PrimitiveType.Triangles,
                (uint)_quad.Indices.Length,
                DrawElementsType.UnsignedInt, null);
        }

        public void Dispose()
        {
            _vbo.Dispose();
            _ebo.Dispose();
            _vao.Dispose();
            _shader.Dispose();
            _texture.Dispose();
        }
    }
}
