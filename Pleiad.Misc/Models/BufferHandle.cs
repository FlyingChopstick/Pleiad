using Pleiad.Misc.Core;

namespace Pleiad.Misc.Models
{
    public class BufferHandle : ValueClass<uint>
    {
        public BufferHandle(uint handle)
        {
            Value = handle;
        }
    }
}