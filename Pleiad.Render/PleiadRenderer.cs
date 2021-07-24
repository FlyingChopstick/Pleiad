using Pleiad.Render.Models;
using Pleiad.Render.Shaders;

namespace Pleiad.Render
{
    public static class PleiadRenderer
    {
        public static PShader Shader { get; set; }


        public static void DrawSprite(PSprite sprite)
        {
            Shader.SetUniform("uModel", sprite.ViewMatrix);
            sprite.Draw(Shader);
        }
    }
}
