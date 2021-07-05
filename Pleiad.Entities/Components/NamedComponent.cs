using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pleiad.Entities.Components
{
    /// <summary>
    /// Basic component for Entity name
    /// </summary>
    public struct NamedComponent : IPleiadComponent
    {
        public string Name;
    }
}
