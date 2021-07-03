using System;
using Pleiad.Misc.Models;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Shaders
{
    public class MergedShader
    {
        public MergedShader(PShader vertex, PShader fragment, GL api)
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


        public static implicit operator uint(MergedShader m) => m.Handle;


        private void CompileShaders(PShader vertex, PShader fragment)
        {
            Vertex = CompileShader(vertex);
            Fragment = CompileShader(fragment);
        }
        private ShaderHandle CompileShader(PShader shader)
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

        private readonly GL _gl;
    }
}
