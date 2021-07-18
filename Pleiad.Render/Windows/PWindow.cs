using System;
using System.Collections.Generic;
using System.Numerics;
using Pleiad.Extensions.Files;
using Pleiad.Render.ControlEvents;
using Pleiad.Render.Models;
using Pleiad.Render.Shaders;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Pleiad.Render.Windows
{
    public sealed class PWindow : IDisposable
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

            _sprite.Dispose();
            //_vao.Unbind();
            //_shader.Delete();
            //_vbo.Delete();
            //_vao.Delete();


            _window.Close();
            Closed?.Invoke();
        }


        public MouseEvents MouseEvents { get; }
        public GamepadEvents GamepadEvents { get; }
        public KeyboardEvents KeyboardEvents { get; }



        private IWindow _window;
        private PShader _shader;
        private GL _gl;

        //private PVertexBufferObject _vbo;
        //private PElementBufferObject _ebo;
        //private PVertexArrayObject<float, uint> _vao;
        //private PShader _shader;


        ////Vertex shaders are run on each vertex.
        //private static readonly string VertexShaderSource = @"
        //#version 330 core //Using version GLSL version 3.3
        //layout (location = 0) in vec4 vPos;

        //void main()
        //{
        //    gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        //}
        //";

        ////Fragment shaders are run on each fragment/pixel of the geometry.
        //private static readonly string FragmentShaderSource = @"
        //#version 330 core
        //out vec4 FragColor;

        //void main()
        //{
        //    FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
        //}
        //";

        //private readonly PMesh _mesh = new()
        //{
        //    //Vertex data, uploaded to the VBO.
        //    Vertices = new float[]
        //    {
        //        //X    Y      Z
        //         0.5f,  0.5f, 0.0f,
        //         0.5f, -0.5f, 0.0f,
        //        -0.5f, -0.5f, 0.0f,
        //        -0.5f,  0.5f, 0.5f,
        //         0.0f,  1,0f, 0.0f
        //    },
        //    //Index data, uploaded to the EBO.
        //    Indices = new uint[]
        //    {
        //        0, 1, 3,
        //        1, 2, 3,
        //        0, 3, 4
        //    }
        //};

        PSprite _sprite;
        //Setup the camera's location, and relative up and right directions
        private static Vector3 CameraPosition = new Vector3(0.0f, 0.0f, 3.0f);
        private static Vector3 CameraTarget = Vector3.Zero;
        private static Vector3 CameraDirection = Vector3.Normalize(CameraPosition - CameraTarget);
        private static Vector3 CameraRight = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, CameraDirection));
        private static Vector3 CameraUp = Vector3.Cross(CameraDirection, CameraRight);
        private unsafe void OnLoad()
        {
            IInputContext _input = _window.CreateInput();
            _input.ConnectionChanged += ConnectInput;

            //ConnectDevices(_input.Gamepads);
            ConnectDevices(_input.Keyboards);
            ConnectDevices(_input.Mice);



            _gl = GL.GetApi(_window);

            PTexture texture = new(_gl, new(@"Textures/texture.png"));
            //vertex shader
            FileContract VertexShaderSource = new("Shaders/shader.vert");
            PShaderSource vertexShader = new(ShaderType.VertexShader, VertexShaderSource);
            //fragment shader
            FileContract FragmentShaderSource = new("Shaders/shader.frag");
            PShaderSource fragmentShader = new(ShaderType.FragmentShader, FragmentShaderSource);
            //merge shaders
            _shader = new(_gl, vertexShader, fragmentShader);
            _sprite = new(_gl, PMesh<float, uint>.Quad, texture, _shader);
            _sprite.Load();
        }
        private unsafe void OnRender(double obj)
        {
            _gl.Clear((uint)ClearBufferMask.ColorBufferBit);

            _sprite.Draw();

            //_sprite.Transform(new()
            //{
            //    Position = new(2.0f, 0.0f, 0.0f)
            //});
            //_sprite.Transform(new()
            //{
            //    Rotation = 20.0f
            //});
            //_sprite.Transform(new()
            //{
            //    Scale = 0.5f
            //});

            var diff = (float)(_window.Time * 100);
            var model = Matrix4x4.CreateRotationY(PTransform.DegreesToRadians(diff))
                * Matrix4x4.CreateRotationX(PTransform.DegreesToRadians(diff));
            var view = Matrix4x4.CreateLookAt(CameraPosition, CameraTarget, CameraUp);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(PTransform.DegreesToRadians(45.0f), 800 / 600, 0.1f, 100.0f);

            _shader.SetUniform("uModel", model);
            _shader.SetUniform("uView", view);
            _shader.SetUniform("uProjection", projection);


            _gl.DrawArrays(PrimitiveType.Triangles, 0, 36);
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



        private void OnUpdate(double deltaTime)
        {
            Updated?.Invoke(deltaTime);
        }
        private void OnClose()
        {
            Closed?.Invoke();
        }


        public void Dispose()
        {
            //_sprite.Dispose();
            _window.Dispose();
        }
    }
}
