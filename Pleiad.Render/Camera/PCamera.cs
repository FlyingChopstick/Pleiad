using System.Numerics;

namespace Pleiad.Render.Camera
{
    public struct PCamera
    {
        public PCamera(Vector3 position, Vector3 target, Vector3 front)
        {
            Position = position;
            Target = target;
            Front = front;
        }

        public Vector3 Position;
        public Vector3 Target;
        public Vector3 Front;

        public Vector3 Direction { get => Vector3.Normalize(Position - Target); }
        public Vector3 Right { get => Vector3.Normalize(Vector3.Cross(Vector3.UnitY, Direction)); }
        public Vector3 Up { get => Vector3.Cross(Direction, Right); }
        public Matrix4x4 ViewMatrix { get => Matrix4x4.CreateLookAt(Position, Target, Up); }

        public void MoveCamera(Vector3 movement)
        {
            Position += movement;
        }

        public static implicit operator Matrix4x4(PCamera c) => c.ViewMatrix;
    }
}
