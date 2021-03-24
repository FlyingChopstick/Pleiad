using Silk.NET.OpenGL;

namespace PleiadRender.Abstractions
{
    public class PVertexArray
    {
        private readonly GL _gl;
        public uint VAO { get; init; }

        public PVertexArray(GL api)
        {
            _gl = api;
            VAO = _gl.GenVertexArray();
            _gl.EnableVertexAttribArray(0);
        }
        public void Bind()
        {
            _gl.BindVertexArray(VAO);
        }
        public void Enable()
        {
            _gl.EnableVertexAttribArray(0);
        }
        public void Disable()
        {
            _gl.DisableVertexAttribArray(0);
        }


        public static implicit operator uint(PVertexArray pva) => pva.VAO;
    }
}
