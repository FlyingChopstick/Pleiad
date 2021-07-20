using System.Numerics;

namespace Pleiad.Entities.Components
{
    /// <summary>
    /// Basic component for Entity rotation
    /// </summary>
    public struct RotationComponent : IPleiadComponent
    {
        public Quaternion rotationDegree;
    }
}
