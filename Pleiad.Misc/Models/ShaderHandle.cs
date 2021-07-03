using Pleiad.Misc.Core;

namespace Pleiad.Misc.Models
{
    public class ShaderHandle : ValueClass<uint>
    {
        public ShaderHandle(uint handle)
        {
            Value = handle;
        }
    }
}
