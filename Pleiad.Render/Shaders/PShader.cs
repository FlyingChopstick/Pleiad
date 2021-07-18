using System;
using Pleiad.Render.Handles;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Shaders
{
    public sealed class PShader : IDisposable
    {
        public PShader(GL api, PShaderSource vertex, PShaderSource fragment)
        {
            _gl = api;
            Handle = new(_gl.CreateProgram());
            CompileShaders(vertex, fragment);
            AttachShaders();
            TryLinkShaders();
            Cleanup();
        }
        public void Use()
        {
            _gl.UseProgram(Handle);
        }
        public void Disable()
        {
            _gl.UseProgram(0);
        }
        public void Delete()
        {
            _gl.DeleteProgram(Handle);
        }


        public ProgramHandle Handle { get; private set; }
        public ShaderHandle Vertex { get; private set; }
        public ShaderHandle Fragment { get; private set; }
        public void SetUniform(string uniformName, int value)
        {
            int location = _gl.GetUniformLocation(Handle, uniformName);
            if (location == -1)
            {
                throw new ArgumentException($"Uniform \"{uniformName}\" not found on shader.");
            }

            _gl.Uniform1(location, value);
        }
        public void SetUniform(string uniformName, float value)
        {
            int location = _gl.GetUniformLocation(Handle, uniformName);
            if (location == -1)
            {
                throw new ArgumentException($"Uniform \"{uniformName}\" not found on shader.");
            }

            _gl.Uniform1(location, value);
        }
        public void SetUniform(string uniformName, double value)
        {
            int location = _gl.GetUniformLocation(Handle, uniformName);
            if (location == -1)
            {
                throw new ArgumentException($"Uniform \"{uniformName}\" not found on shader.");
            }

            _gl.Uniform1(location, value);
        }


        public static implicit operator uint(PShader m) => m.Handle;


        private void CompileShaders(PShaderSource vertex, PShaderSource fragment)
        {
            Vertex = CompileShader(vertex);
            Fragment = CompileShader(fragment);
        }
        private ShaderHandle CompileShader(PShaderSource shader)
        {
            ShaderHandle handle = new(_gl.CreateShader(shader.Type));
            _gl.ShaderSource(handle, shader.Source);
            _gl.CompileShader(handle);

            string infoLog = _gl.GetShaderInfoLog(handle);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new ArgumentException($"Error compiling {shader.Type} shader\n{infoLog}");
            }

            return handle;
        }
        private void AttachShaders()
        {
            _gl.AttachShader(Handle, Vertex);
            _gl.AttachShader(Handle, Fragment);
        }
        private void TryLinkShaders()
        {
            _gl.LinkProgram(Handle);
            _gl.GetProgram(Handle, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                throw new ArgumentException($"Error linking shader {_gl.GetProgramInfoLog(Handle)}");
            }
        }
        private void Cleanup()
        {
            _gl.DetachShader(Handle, Vertex);
            _gl.DetachShader(Handle, Fragment);
            _gl.DeleteShader(Vertex);
            _gl.DeleteShader(Fragment);
        }

        public void Dispose()
        {
            _gl.DeleteProgram(Handle);
        }

        private readonly GL _gl;
    }
}
