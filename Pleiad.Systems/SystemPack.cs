using Pleiad.Entities;
using System.Collections.Generic;
using System.Reflection;

namespace Pleiad.Systems
{
    internal struct SystemPack
    {
        public SystemPack(Dictionary<object, MethodInfo> systemsInfo, IPleiadComponent[] query = default)
        {
            Systems = systemsInfo;
            Query = query;
        }
        public Dictionary<object, MethodInfo> Systems { get; private set; }
        public IPleiadComponent[] Query { get; }
    }
}
