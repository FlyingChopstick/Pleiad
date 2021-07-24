using System;
using System.Collections.Generic;
using Pleiad.Render.ControlEvents;
using Pleiad.Render.Shaders;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Pleiad.Render.Windows
{
    public sealed class PWindow : IDisposable
    {
        public delegate void UpdatedDelegate(double deltaTime);
        public delegate void ClosedDelegate();
        public delegate void WindowLoadDelegate();
        public delegate void RenderDelegate(double obj);

        public event UpdatedDelegate Updated;
        public event ClosedDelegate Closed;
        public event WindowLoadDelegate Load;
        public event RenderDelegate Render;


        public PWindow(IPWindowOptions options)
        {
            _window = Window.Create(options.SilkOptions);

            _window.Load += OnLoad;
            _window.Render += OnRender;
            _window.Update += OnUpdate;
            _window.Closing += OnClose;
            _window.Resize += OnResize;


            MouseEvents = new();
            GamepadEvents = new();
            KeyboardEvents = new();
        }


        public GL Api;
        public PShader Shader { get; set; }
        public bool IsClosing { get => _window.IsClosing; }
        public int Width { get => _window.Size.X; }
        public int Height { get => _window.Size.Y; }
        public MouseEvents MouseEvents { get; }
        public GamepadEvents GamepadEvents { get; }
        public KeyboardEvents KeyboardEvents { get; }



        public void Run()
        {
            _window.Run();
        }
        public void Close()
        {
            Api.BindBuffer(GLEnum.ArrayBuffer, 0);

            //_sprite.Dispose();
            //_vao.Unbind();
            //_shader.Delete();
            //_vbo.Delete();
            //_vao.Delete();


            _window.Close();
            Closed?.Invoke();
        }



        private readonly IWindow _window;


        private unsafe void OnLoad()
        {
            IInputContext _input = _window.CreateInput();
            _input.ConnectionChanged += ConnectInput;

            //ConnectDevices(_input.Gamepads);
            ConnectDevices(_input.Keyboards);
            ConnectDevices(_input.Mice);



            Api = GL.GetApi(_window);

            Load?.Invoke();

            //PTexture texture = new(Api, new(@"Textures/texture.png"));
            ////vertex shader
            //FileContract VertexShaderSource = new("Shaders/shader.vert");
            //PShaderSource vertexShader = new(ShaderType.VertexShader, VertexShaderSource);
            ////fragment shader
            //FileContract FragmentShaderSource = new("Shaders/shader.frag");
            //PShaderSource fragmentShader = new(ShaderType.FragmentShader, FragmentShaderSource);
            ////merge shaders
            //Shader = new(Api, vertexShader, fragmentShader);
            //_sprite = new(Api, PMesh<float, uint>.Quad, texture, Shader);
            //_sprite.Load();


            //_sprite.Transform(new()
            //{
            //    Position = new(0.5f, 0.0f, 0.0f)
            //});
            //_sprite.Transform(new()
            //{
            //    Scale = 0.5f
            //});
        }
        private void OnUpdate(double deltaTime)
        {
            Updated?.Invoke(deltaTime);
        }
        private unsafe void OnRender(double obj)
        {
            Api.Clear((uint)ClearBufferMask.ColorBufferBit);

            Render?.Invoke(obj);
        }
        private void OnResize(Vector2D<int> obj)
        {
        }
        private void OnClose()
        {
            Closed?.Invoke();
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

        public void Dispose()
        {
            //_sprite.Dispose();
            _window.Dispose();
        }
    }
}
