using System;
using Pleiad.Misc.Models;
using Pleiad.Render.Interfaces;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Shaders
{
    public class PShader : IShader
    {
        public PShader(ShaderType type, string source, GL api)
        {
            _gl = api;
            Type = type;
            Source = source;

            Handle = null;
            IsCompiled = false;
        }

        public ShaderType Type { get; init; }
        public ShaderHandle Handle { get; private set; }
        public string Source { get; init; }
        public bool IsCompiled { get; private set; }

        public ShaderHandle Compile()
        {
            Handle = new(_gl.CreateShader(Type));
            _gl.ShaderSource(Handle, Source);
            _gl.CompileShader(Handle);

            string infoLog = _gl.GetShaderInfoLog(Handle);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new ArgumentException($"Error compiling {Type} shader\n{infoLog}");
            }

            IsCompiled = true;
            return Handle;
        }
        public static implicit operator uint(PShader s) => s.Handle;


        private GL _gl;
    }
}
