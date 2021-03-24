using System;

namespace PleiadRender.Abstractions
{
    [Serializable]
    public struct PShapeData
    {
        public float[] Vertices { get; init; }
        public uint[] Indices { get; init; }

    }
}
