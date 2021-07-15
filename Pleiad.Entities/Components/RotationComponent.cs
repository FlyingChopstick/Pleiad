using System.Numerics;

namespace Pleiad.Entities.Components
{
    /// <summary>
    /// Basic component for Entity rotation
    /// </summary>
    struct RotationComponent : IPleiadComponent
    {
        public Quaternion rotation;
    }
}
