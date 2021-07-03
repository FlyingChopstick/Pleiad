using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pleiad.Misc.Models;
using Silk.NET.OpenGL;

namespace Pleiad.Render.Arrays
{
    public class PVertexArray
    {
        public PVertexArray(GL api)
        {
            _gl = api;
            Handle = new(_gl.GenVertexArray());
        }
        public void Bind()
        {
            _gl.BindVertexArray(Handle);
        }
        public void Unbind()
        {
            _gl.BindVertexArray(0);
        }
        public void Delete()
        {
            _gl.DeleteVertexArray(Handle);
        }

        public VertexArrayHandle Handle { get; }


        private GL _gl;
    }
}
