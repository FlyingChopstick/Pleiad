using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;

namespace PleiadRender.Abstractions
{
    public class PProgram
    {
        private readonly GL _gl;
        private readonly List<PShader> _shaders;
        public uint Program { get; init; }


        public PProgram(GL api)
        {
            _gl = api;
            Program = _gl.CreateProgram();
            _shaders = new List<PShader>();
        }


        public void AttachShader(PShader shader)
        {
            _gl.AttachShader(Program, shader);
            _shaders.Add(shader);
        }
        public void Link()
        {
            _gl.LinkProgram(Program);
            //check
            _gl.GetProgram(Program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                throw new ArgumentException($"Error linking shader\n{_gl.GetProgramInfoLog(Program)}");
            }
        }
        public void Clean()
        {
            foreach (var shader in _shaders)
            {
                _gl.DetachShader(Program, shader);
                _gl.DeleteShader(shader);
            }
            _shaders.Clear();
        }
        public void Use()
        {
            _gl.UseProgram(Program);
        }

        public static implicit operator uint(PProgram pp) => pp.Program;
    }
}
