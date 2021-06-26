using PleiadEntities;
using System.Collections.Generic;
using System.Reflection;

namespace PleiadSystems
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
