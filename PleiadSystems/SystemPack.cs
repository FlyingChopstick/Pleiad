using PleiadEntities;
using System.Collections.Generic;
using System.Reflection;

namespace PleiadSystems
{
    internal enum SystemIs
    {
        Simple = 0,
        For = 1
    }
    internal struct SystemPack
    {
        public SystemPack(SystemIs type, Dictionary<object, MethodInfo> systemsInfo, IPleiadComponent[] query = default)
        {
            Type = type;
            Systems = systemsInfo;
            Query = query;
        }

        public SystemIs Type { get; }
        public Dictionary<object, MethodInfo> Systems { get; private set; }
        public IPleiadComponent[] Query { get; }
    }
}
