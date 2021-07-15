using Pleiad.Render.Interfaces;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Shaders
{
    public struct PShader : IShader
    {
        public PShader(ShaderType type, string source)
        {
            Type = type;
            Source = source;
        }

        public ShaderType Type { get; init; }
        public string Source { get; init; }
    }
}
