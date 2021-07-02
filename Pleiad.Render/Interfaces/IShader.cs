using Pleiad.Misc.Models;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Interfaces
{
    public interface IShader
    {
        ShaderType Type { get; init; }
        ShaderHandle Handle { get; }
        string Source { get; init; }
        bool IsCompiled { get; }

        /// <summary>
        /// Compiles shader and returns it's handle
        /// </summary>
        /// <returns>Shader handle</returns>
        ShaderHandle Compile();
    }
}
