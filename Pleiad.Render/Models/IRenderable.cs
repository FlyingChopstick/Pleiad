using Pleiad.Render.Shaders;

namespace Pleiad.Render.Models
{
    public interface IRenderable
    {
        void Load();
        void Draw(PShader shader);
    }
}
