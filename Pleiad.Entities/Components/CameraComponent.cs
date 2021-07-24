using System.Numerics;
using Pleiad.Render.Camera;

namespace Pleiad.Entities.Components
{
    public struct CameraComponent : IPleiadComponent
    {
        public Vector3 Position;
        public Vector3 Target;


        public static implicit operator PCamera(CameraComponent c)
            => new(c.Position, c.Target, new(0.0f, 0.0f, -1.0f));
        public static implicit operator CameraComponent(PCamera c)
            => new() { Position = c.Position, Target = c.Target };
    }
}
