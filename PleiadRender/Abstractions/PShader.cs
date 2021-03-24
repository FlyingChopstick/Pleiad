using Silk.NET.OpenGL;
using System;
using System.IO;

namespace PleiadRender.Abstractions
{
    public class PShader
    {
        private readonly GL _gl;
        private readonly ShaderType _shaderType;
        private string _shaderSource;

        public uint Shader { get; init; }

        public PShader(GL api, ShaderType shaderType)
        {
            _gl = api;
            _shaderType = shaderType;
            _shaderSource = string.Empty;

            Shader = _gl.CreateShader(_shaderType);
        }

        public void ReadSourceFile(string filename)
        {
            if (File.Exists(filename))
            {
                _shaderSource = File.ReadAllText(filename);
            }
            else
            {
                throw new FileNotFoundException($"{_shaderType} - Could not find source file");
            }
        }
        public void Compile()
        {
            if (_shaderSource != string.Empty)
            {
                _gl.ShaderSource(Shader, _shaderSource);
                _gl.CompileShader(Shader);
                CheckShader();
            }
            else
            {
                throw new ArgumentException($"{_shaderType} - Invalid source string");
            }
        }

        private void CheckShader()
        {
            string infolog = _gl.GetShaderInfoLog(Shader);
            if (!string.IsNullOrWhiteSpace(infolog))
            {
                ThrowException(infolog);
                return;
            }
        }
        private void ThrowException(string infolog)
        {
            switch (_shaderType)
            {
                case ShaderType.FragmentShader:
                    {
                        throw new ArgumentException($"Error compiling fragment shader\n{infolog}");
                    }
                case ShaderType.VertexShader:
                    {
                        throw new ArgumentException($"Error compiling vertex shader\n{infolog}");
                    }
                default:
                    {
                        throw new ArgumentException($"Error compiling shader\n{infolog}");
                    }
            }
        }


        public static implicit operator uint(PShader ps) => ps.Shader;
    }
}
