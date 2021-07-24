﻿using Silk.NET.OpenGL;

namespace Pleiad.Systems.Interfaces
{
    public interface IRenderSystem
    {
        void Load();
        void Render(double obj);
    }
}