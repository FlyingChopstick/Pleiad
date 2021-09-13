using Pleiad.Common.Model;

namespace Pleiad.Render.Handles
{
    public class TextureHandle : ValueClass<uint>
    {
        public TextureHandle(uint handle)
        {
            Value = handle;
        }
    }
}
