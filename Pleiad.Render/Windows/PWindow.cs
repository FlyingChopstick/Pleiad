using System;
using System.Collections.Generic;
using Pleiad.Render.Arrays;
using Pleiad.Render.Buffers;
using Pleiad.Render.ControlEvents;
using Pleiad.Render.Models;
using Pleiad.Render.Shaders;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Pleiad.Render.Windows
{
    public class PWindow : IDisposable
    {
        public delegate bool UpdatedDelegate(double deltaTime);
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

            MouseEvents = new();
            GamepadEvents = new();
            KeyboardEvents = new();
        }
        public void Run()
        {
            _window.Run();
        }
        public void Close()
        {
            _gl.BindBuffer(GLEnum.ArrayBuffer, 0);

            _vao.Unbind();
            _shader.Delete();
            _vbo.Delete();
            _vao.Delete();


            _window.Close();
            Closed?.Invoke();
        }


        public MouseEvents MouseEvents { get; }
        public GamepadEvents GamepadEvents { get; }
        public KeyboardEvents KeyboardEvents { get; }



        private IWindow _window;
        private IInputContext _input;
        private GL _gl;

        private VertexBuffer _vbo;
        private ElementBuffer _ebo;
        private PVertexArray _vao;
        private MergedShader _shader;

        private bool _disposedValue;

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

        private readonly PMesh _mesh = new()
        {
            //Vertex data, uploaded to the VBO.
            Vertices = new float[]
            {
                //X    Y      Z
                 0.5f,  0.5f, 0.0f,
                 0.5f, -0.5f, 0.0f,
                -0.5f, -0.5f, 0.0f,
                -0.5f,  0.5f, 0.5f,
                 0.0f,  1,0f, 0.0f
            },
            //Index data, uploaded to the EBO.
            Indices = new uint[]
            {
                0, 1, 3,
                1, 2, 3,
                0, 3, 4
            }
        };

        private unsafe void OnLoad()
        {
            _input = _window.CreateInput();
            _input.ConnectionChanged += ConnectInput;

            //ConnectDevices(_input.Gamepads);
            ConnectDevices(_input.Keyboards);
            ConnectDevices(_input.Mice);



            _gl = GL.GetApi(_window);

            _vao = new(_gl);
            _vao.Bind();

            //vertex buffer
            _vbo = _mesh.GenerateVertexBuffer(BufferUsageARB.StaticDraw, _gl);
            _vbo.Bind();

            //element buffer
            _ebo = _mesh.GenerateElementBuffer(BufferUsageARB.StaticDraw, _gl);
            _ebo.Bind();

            //vertex shader
            PShader vertexShader = new(ShaderType.VertexShader, VertexShaderSource);
            //fragment shader
            PShader fragmentShader = new(ShaderType.FragmentShader, FragmentShaderSource);
            //merge shaders
            _shader = new(vertexShader, fragmentShader, _gl);

            _gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            _gl.EnableVertexAttribArray(0);
        }


        private void ConnectDevices<T>(IReadOnlyCollection<T> devices) where T : IInputDevice
        {
            foreach (var device in devices)
            {
                if (!device.IsConnected) continue;
                //if (device is IGamepad)
                //{
                //    //TODO For some reason, disconnecting the gamepad freezes the window.
                //    //The window cannot be closed, save for full OS reboot.
                //    Console.WriteLine("WARNING: Disconnecting the gamepad during runtime breaks the window, " +
                //        "throwing it into the infinite loop. Connection of gamepads is disabled.");

                //    continue;
                //}

                ConnectInput(device, device.IsConnected);
            }
        }
        private void ConnectInput(IInputDevice device, bool isConnected)
        {
            var connectionState = isConnected ? "connected" : "disconnected";

            if (device is IGamepad gamepad)
            {
                ConnectGamepad(isConnected, connectionState, gamepad);
            }
            else if (device is IKeyboard keyboard)
            {
                ConnectKeyboard(isConnected, connectionState, keyboard);
            }
            else if (device is IMouse mouse)
            {
                ConnectMouse(isConnected, connectionState, mouse);
            }
            else
            {
                Console.WriteLine("Unknown input device detected");
            }
        }

        private void ConnectMouse(bool isConnected, string connectionState, IMouse mouse)
        {
            Console.WriteLine($"Mouse {mouse.Index} {connectionState}");

            if (isConnected)
            {
                mouse.MouseUp += MouseEvents.Mouse_MouseUp;
                mouse.MouseDown += MouseEvents.Mouse_MouseDown; ;
                mouse.Click += MouseEvents.Mouse_Click; ;
                mouse.DoubleClick += MouseEvents.Mouse_DoubleClick; ;
                mouse.Scroll += MouseEvents.Mouse_Scroll; ;
                mouse.MouseMove += MouseEvents.Mouse_MouseMove; ;
            }
            else
            {
                mouse.MouseUp -= MouseEvents.Mouse_MouseUp;
                mouse.MouseDown -= MouseEvents.Mouse_MouseDown;
                mouse.Click -= MouseEvents.Mouse_Click;
                mouse.DoubleClick -= MouseEvents.Mouse_DoubleClick;
                mouse.Scroll -= MouseEvents.Mouse_Scroll;
                mouse.MouseMove -= MouseEvents.Mouse_MouseMove;
            }
        }
        private void ConnectKeyboard(bool isConnected, string connectionState, IKeyboard keyboard)
        {
            Console.WriteLine($"Keyboard {keyboard.Index} {connectionState}");

            if (isConnected)
            {
                keyboard.KeyDown += KeyboardEvents.Keyboard_KeyDown;
                keyboard.KeyUp += KeyboardEvents.Keyboard_KeyUp;
                keyboard.KeyChar += KeyboardEvents.Keyboard_KeyChar;
            }
            else
            {
                keyboard.KeyDown -= KeyboardEvents.Keyboard_KeyDown;
                keyboard.KeyUp -= KeyboardEvents.Keyboard_KeyUp;
                keyboard.KeyChar -= KeyboardEvents.Keyboard_KeyChar;
            }
        }
        private void ConnectGamepad(bool isConnected, string connectionState, IGamepad gamepad)
        {
            Console.WriteLine($"Gamepad {gamepad.Index} {connectionState}");

            if (isConnected)
            {
                gamepad.Deadzone = new(0.2f, DeadzoneMethod.AdaptiveGradient);
                gamepad.ButtonDown += GamepadEvents.Gamepad_ButtonDown;
                gamepad.ButtonUp += GamepadEvents.Gamepad_ButtonUp;
                gamepad.ThumbstickMoved += GamepadEvents.Gamepad_ThumbstickMoved;
                gamepad.TriggerMoved += GamepadEvents.Gamepad_TriggerMoved;
            }
            else
            {
                gamepad.ButtonDown -= GamepadEvents.Gamepad_ButtonDown;
                gamepad.ButtonUp -= GamepadEvents.Gamepad_ButtonUp;
                gamepad.ThumbstickMoved -= GamepadEvents.Gamepad_ThumbstickMoved;
                gamepad.TriggerMoved -= GamepadEvents.Gamepad_TriggerMoved;
            }
        }


        private unsafe void OnRender(double obj)
        {
            _gl.Clear((uint)ClearBufferMask.ColorBufferBit);

            _vao.Bind();

            _shader.Use();

            _gl.DrawElements(PrimitiveType.Triangles, (uint)_mesh.Indices.Length, DrawElementsType.UnsignedInt, null);
        }
        private void OnUpdate(double deltaTime)
        {
            Updated?.Invoke(deltaTime);
        }
        private void OnClose()
        {
            Closed?.Invoke();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _window.Dispose();
                }

                _window = null;
                _disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
