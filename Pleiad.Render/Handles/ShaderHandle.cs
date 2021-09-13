using Pleiad.Common.Model;

namespace Pleiad.Render.Handles
{
    public class ShaderHandle : ValueClass<uint>
    {
        public ShaderHandle(uint handle)
        {
            Value = handle;
        }
    }
}
