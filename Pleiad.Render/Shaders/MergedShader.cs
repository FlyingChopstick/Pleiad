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

            _gl.AttachShader(Handle, vertex);
            _gl.AttachShader(Handle, fragment);

            _gl.LinkProgram(Handle);

            _gl.GetProgram(Handle, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                throw new ArgumentException($"Error linking shader {_gl.GetProgramInfoLog(Handle)}");
            }
            _gl.DetachShader(Handle, vertex);
            _gl.DetachShader(Handle, fragment);
            _gl.DeleteShader(vertex);
            _gl.DeleteShader(fragment);
        }

        public ProgramHandle Handle { get; }
        public static implicit operator uint(MergedShader m) => m.Handle;


        private GL _gl;
    }
}
