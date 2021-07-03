using Silk.NET.OpenGL;

namespace Pleiad.Render.Interfaces
{
    public interface IShader
    {
        ShaderType Type { get; init; }
        string Source { get; init; }
    }
}
