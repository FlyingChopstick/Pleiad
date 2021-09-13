using Pleiad.Common.Model;

namespace Pleiad.Render.Handles
{
    public class BufferHandle : ValueClass<uint>
    {
        public BufferHandle(uint handle)
        {
            Value = handle;
        }
    }
}