﻿using System.Text;
using Pleiad.Extensions.Files;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Shaders
{
    public class PShaderSource
    {
        public PShaderSource(ShaderType type, string source)
        {
            Type = type;
            Source = source;
        }
        public PShaderSource(ShaderType type, FileContract sourceFile)
        {
            Type = type;
            var src = sourceFile.ReadLines();
            StringBuilder sb = new();
            for (int i = 0; i < src.Length; i++)
            {
                sb.Append(src[i]);
                sb.Append('\n');
            }

            Source = sb.ToString();
        }

        public ShaderType Type { get; init; }
        public string Source { get; init; }
    }
}
