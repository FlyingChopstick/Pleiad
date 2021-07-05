using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pleiad.Entities.Components
{
    /// <summary>
    /// Basic component for Entity location
    /// </summary>
    public struct LocationComponent : IPleiadComponent
    {
        public float X;
        public float Y;
    }
}
