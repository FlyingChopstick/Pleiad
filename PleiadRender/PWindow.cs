using PleiadRender.Abstractions;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace PleiadRender
{
    public class PWindow
    {
        public delegate bool UpdatedDelegate();
        public delegate void ClosedDelegate();

        public event UpdatedDelegate Updated;
        public event ClosedDelegate Closed;

        public bool IsClosing => _window.IsClosing;

        public PWindow(IPWindowOptions options)
        {
            _window = Window.Create(options.SilkOptions);

            _window.Load += OnLoad;
            _window.Render += OnRender;
            _window.Update += OnUpdate;
            _window.Closing += OnClose;
        }
        public void Run()
        {
            _window.Run();
        }
        public void Close()
        {
            _window.Close();
            Closed?.Invoke();
        }

        private IWindow _window;
        private GL _gl;

        private PBuffer _vbo;
        private PBuffer _ebo;
        private PProgram _shader;
        private PVertexArray _vao;

        //Vertex data, uploaded to the VBO.
        private readonly float[] Vertices =
        {
            //X    Y      Z
             0.5f,  0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
            -0.5f, -0.5f, 0.0f,
            -0.5f,  0.5f, 0.5f,
             0.0f,  1,0f, 0.0f
        };

        //Index data, uploaded to the EBO.
        private readonly uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3,
            0, 3, 4
        };

        private unsafe void OnLoad()
        {
            _gl = GL.GetApi(_window);

            _vao = new PVertexArray(_gl);
            _vao.Bind();

            //vertex buffer
            _vbo = new(_gl, BufferType.Vertex);
            _vbo.SetData(Vertices);
            //element buffer
            _ebo = new(_gl, BufferType.Element);
            _ebo.SetData(Indices);

            //vertex shader
            PShader vShader = new(_gl, ShaderType.VertexShader);
            vShader.ReadSourceFile(@"Shaders\vertex.vert");
            vShader.Compile();
            //fragment shader
            PShader fShader = new(_gl, ShaderType.FragmentShader);
            fShader.ReadSourceFile(@"Shaders\fragment.frag");
            fShader.Compile();

            //merge shaders
            _shader = new PProgram(_gl);
            _shader.AttachShader(vShader);
            _shader.AttachShader(fShader);
            _shader.Link();
            _shader.Clean();


            //stays as is
            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);


            _vao.Enable();
        }
        private void OnRender(double obj)
        {
            Clear();

            _vao.Bind();
            _shader.Use();

            Draw();
        }
        private void OnUpdate(double obj)
        {
            Updated?.Invoke();
        }
        private void OnClose()
        {
            Closed?.Invoke();
        }


        private void Clear()
        {
            _gl.Clear((uint)ClearBufferMask.ColorBufferBit);
        }
        private unsafe void Draw()
        {
            _gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
        }
    }
}
