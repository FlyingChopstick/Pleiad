using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;

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

        private uint _vbo;
        private uint _ebo;
        private uint _vao;
        private uint _shader;

        //Vertex shaders are run on each vertex.
        private static readonly string VertexShaderSource = @"
        #version 330 core //Using version GLSL version 3.3
        layout (location = 0) in vec4 vPos;
        
        void main()
        {
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }
        ";

        //Fragment shaders are run on each fragment/pixel of the geometry.
        private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
        }
        ";

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

            _vao = _gl.GenVertexArray();
            _gl.BindVertexArray(_vao);

            //vertex buffer
            _vbo = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            fixed (void* v = &Vertices[0])
            {
                //set buffer data
                _gl.BufferData(
                    BufferTargetARB.ArrayBuffer,
                    (nuint)(Vertices.Length * sizeof(uint)),
                    v,
                    BufferUsageARB.StaticDraw);
            }

            //element buffer
            _ebo = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
            fixed (void* i = &Indices[0])
            {
                _gl.BufferData(
                    BufferTargetARB.ElementArrayBuffer,
                    (nuint)(Indices.Length * sizeof(uint)),
                    i,
                    BufferUsageARB.StaticDraw);
            }

            //vertex shader
            uint vShader = _gl.CreateShader(ShaderType.VertexShader);
            _gl.ShaderSource(vShader, VertexShaderSource);
            _gl.CompileShader(vShader);
            //check vShader
            string infolog = _gl.GetShaderInfoLog(vShader);
            if (!string.IsNullOrWhiteSpace(infolog))
            {
                Console.WriteLine($"Error compiling vertex shader\n{infolog}");
            }

            //fragment shader
            uint fShader = _gl.CreateShader(ShaderType.FragmentShader);
            _gl.ShaderSource(fShader, FragmentShaderSource);
            _gl.CompileShader(fShader);
            //check fShader
            infolog = _gl.GetShaderInfoLog(fShader);
            if (!string.IsNullOrWhiteSpace(infolog))
            {
                Console.WriteLine($"Error compiling fragment shader\n{infolog}");
            }

            //merge shaders
            _shader = _gl.CreateProgram();
            _gl.AttachShader(_shader, vShader);
            _gl.AttachShader(_shader, fShader);
            _gl.LinkProgram(_shader);
            //check
            _gl.GetProgram(_shader, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {_gl.GetProgramInfoLog(_shader)}");
            }

            //cleanup
            _gl.DetachShader(_shader, vShader);
            _gl.DetachShader(_shader, fShader);
            _gl.DeleteShader(vShader);
            _gl.DeleteShader(fShader);

            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            _gl.EnableVertexAttribArray(0);
        }
        private unsafe void OnRender(double obj)
        {
            _gl.Clear((uint)ClearBufferMask.ColorBufferBit);

            _gl.BindVertexArray(_vao);
            _gl.UseProgram(_shader);

            _gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
        }
        private void OnUpdate(double obj)
        {
            Updated?.Invoke();
        }
        private void OnClose()
        {
            Closed?.Invoke();
        }
    }
}
