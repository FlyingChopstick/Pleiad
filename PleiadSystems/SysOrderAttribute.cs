using System;

namespace PleiadSystems
{
    public sealed class SysOrderAttribute : Attribute
    {
        public SysOrderAttribute(LoadOrder order = LoadOrder.LoadLast)
        {
            Order = order;
        }

        public LoadOrder Order { get; private set; }
    }
}
