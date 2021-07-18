using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pleiad.Misc.Core;

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
