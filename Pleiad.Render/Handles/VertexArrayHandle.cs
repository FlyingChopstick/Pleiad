using Pleiad.Common.Model;

namespace Pleiad.Render.Handles
{
    public class VertexArrayHandle : ValueClass<uint>
    {
        public VertexArrayHandle(uint handle)
        {
            Value = handle;
        }
    }
}
