using Pleiad.Common.Model;

namespace Pleiad.Render.Handles
{
    public class ProgramHandle : ValueClass<uint>
    {
        public ProgramHandle(uint handle)
        {
            Value = handle;
        }
    }
}
