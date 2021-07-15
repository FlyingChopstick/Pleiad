using System.Numerics;

namespace Pleiad.Entities.Components
{
    /// <summary>
    /// Basic component for Entity location
    /// </summary>
    public struct LocationComponent : IPleiadComponent
    {
        public Vector2 location;
    }
}
