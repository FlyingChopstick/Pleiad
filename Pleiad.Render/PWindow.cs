﻿using System;
using System.Collections.Generic;
using Pleiad.Render.ControlEvents;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Pleiad.Render
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
            _gl.BindVertexArray(0);
            _gl.UseProgram(0);
            _gl.DeleteBuffer(_vbo);
            _gl.DeleteVertexArray(_vao);
            _gl.DeleteProgram(_shader);

            _window.Close();
            Closed?.Invoke();
        }


        public MouseEvents MouseEvents { get; }
        public GamepadEvents GamepadEvents { get; }
        public KeyboardEvents KeyboardEvents { get; }



        private IWindow _window;
        private IInputContext _input;
        private GL _gl;

        private uint _vbo;
        private uint _ebo;
        private uint _vao;
        private uint _shader;
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
            _input = _window.CreateInput();
            _input.ConnectionChanged += ConnectInput;

            ConnectDevices(_input.Gamepads);
            ConnectDevices(_input.Keyboards);
            ConnectDevices(_input.Mice);



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

            _gl.BindVertexArray(_vao);
            _gl.UseProgram(_shader);

            _gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
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
