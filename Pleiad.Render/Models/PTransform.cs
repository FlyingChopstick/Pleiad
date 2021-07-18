using System;
using System.Numerics;

namespace Pleiad.Render.Models
{
    public sealed class PTransform
    {
        public static float DegreesToRadians(float degrees) => MathF.PI / 180f * degrees;



        public Vector3 Position { get; set; } = Vector3.Zero;
        public float Scale { get; set; } = 1.0f;
        public Quaternion Rotation { get; set; } = Quaternion.Identity;

        public Matrix4x4 ViewMatrix => Matrix4x4.Identity
            * Matrix4x4.CreateRotationX(DegreesToRadians(Rotation.X))
            * Matrix4x4.CreateRotationY(DegreesToRadians(Rotation.Y))
            * Matrix4x4.CreateScale(Scale)
            * Matrix4x4.CreateTranslation(Position);

    }
}
