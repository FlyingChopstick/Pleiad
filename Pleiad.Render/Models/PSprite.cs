using System;
using System.Collections.Generic;
using System.Numerics;
using Pleiad.Render.Arrays;
using Pleiad.Render.Shaders;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Models
{
    public sealed class PSprite : IDisposable, IRenderable
    {
        private readonly GL _gl;
        private readonly PTexture _texture;
        //private readonly PShader _shader;
        private readonly PMesh<float, uint> _mesh;
        private PBufferObject<float> _vbo;
        private PBufferObject<uint> _ebo;
        private PVertexArrayObject<float, uint> _vao;
        private Queue<PTransform> _transforms = new();

        public Matrix4x4 ViewMatrix;
        public Vector3 Position { get; set; } = new(0.0f);


        public PSprite(GL api, PMesh<float, uint> mesh, PTexture texture)
        {
            _gl = api;

            _mesh = mesh;
            _texture = texture;
            //_shader = shader;

            ViewMatrix = Matrix4x4.Identity;
            //_shader.SetUniform("uModel", ViewMatrix);
        }
        public unsafe void Load()
        {
            _ebo = _mesh.GenerateElementBuffer(BufferUsageARB.StaticDraw, _gl);
            _vbo = _mesh.GenerateVertexBuffer(BufferUsageARB.StaticDraw, _gl);

            _vao = new(_gl, _vbo, _ebo);
            _vao.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, 5, 0);
            _vao.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, 5, 3);
        }

        public void Translate(Vector2 newPosition)
        {
            Transform(new()
            {
                Position = new(newPosition, 0.0f)
            });
        }
        public void Rotate(float degrees)
        {
            RotateRadians(PTransform.DegreesToRadians(degrees));
        }
        public void RotateRadians(float radians)
        {
            Transform(new()
            {
                Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, radians)
            });
        }
        public void Scale(float scale)
        {
            Transform(new()
            {
                Scale = scale
            });
        }
        public void Transform(PTransform transform)
        {
            _transforms.Enqueue(transform);
        }


        public unsafe void Draw(PShader shader)
        {
            _vao.Bind();
            shader.Use();
            _texture.Bind();
            shader.SetUniform("uTexture0", 0);


            while (_transforms.Count > 0)
            {
                ViewMatrix *= _transforms.Dequeue().ViewMatrix;
                shader.SetUniform("uModel", ViewMatrix);
            }
            DrawElements();
        }


        //private unsafe void ApplyTransforms(PShader shader)
        //{
        //    while (_transforms.Count > 0)
        //    {
        //        shader.SetUniform("uModel", _transforms.Dequeue().ViewMatrix);
        //        DrawElements();
        //    }
        //}
        private unsafe void DrawElements()
        {
            _gl.DrawElements(
                PrimitiveType.Triangles,
                (uint)_mesh.Indices.Length,
                DrawElementsType.UnsignedInt, null);
        }

        public void Dispose()
        {
            _vbo.Dispose();
            _ebo.Dispose();
            _vao.Dispose();
            //_shader.Dispose();
            _texture.Dispose();
        }
    }
}
