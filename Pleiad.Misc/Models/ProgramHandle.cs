using Pleiad.Misc.Core;

namespace Pleiad.Misc.Models
{
    public class ProgramHandle : ValueClass<uint>
    {
        public ProgramHandle(uint handle)
        {
            Value = handle;
        }
    }
}
