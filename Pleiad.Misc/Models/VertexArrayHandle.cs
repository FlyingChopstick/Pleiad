using Pleiad.Misc.Core;

namespace Pleiad.Misc.Models
{
    public class VertexArrayHandle : ValueClass<uint>
    {
        public VertexArrayHandle(uint handle)
        {
            Value = handle;
        }
    }
}
