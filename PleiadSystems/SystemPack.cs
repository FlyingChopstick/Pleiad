using PleiadEntities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PleiadSystems
{

    public enum LoadOrder
    {
        LoadFirst = 0,
        LoadMiddle = 1,
        LoadLast = 2
    }

    internal struct SystemData
    {
        public SystemData(
            Type systemType,
            object systemObject,
            MethodInfo systemMethod,
            LoadOrder loadOrder
            )
        {
            SystemType = systemType;
            SystemObject = systemObject;
            SystemMethod = systemMethod;
            LoadOrder = loadOrder;
        }

        public Type SystemType { get; }
        public object SystemObject { get; }
        public MethodInfo SystemMethod { get; }
        public LoadOrder LoadOrder { get; }
    }
}
