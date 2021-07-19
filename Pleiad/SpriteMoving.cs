using System;
using System.Numerics;
using Pleiad.Entities;
using Pleiad.Entities.Components;
using Pleiad.Extensions.Files;
using Pleiad.Input;
using Pleiad.Render.Models;
using Pleiad.Render.Shaders;
using Pleiad.Systems.Interfaces;
using Pleiad.Tasks;
using Pleiad.Worlds;
using Silk.NET.Input;
using Silk.NET.OpenGL;

public class SpriteEntityAdditionSystem : IPleiadSystem, IRegisterInput
{
    static bool _shouldAdd = false;
    static bool _isLoaded = false;

    static GL _gl;
    static PTexture _texture;
    static PShader _shader;
    static PSprite _sprite;
    static EntityTemplate _template;


    public void Cycle(double dTime)
    {
        if (_shouldAdd)
        {
            if (!_isLoaded)
            {
                LoadTemplate();
            }

            World.ActiveWorld.EntityManager.AddEntity(_template);
        }
    }

    private void LoadTemplate()
    {
        _gl = World.ActiveWorld.SystemsManager.Api;

        // texture
        _texture = new(_gl, new(@"Textures/texture.png"));

        // shaders
        //vertex shader
        FileContract VertexShaderSource = new("Shaders/shader.vert");
        PShaderSource vertexShader = new(ShaderType.VertexShader, VertexShaderSource);
        //fragment shader
        FileContract FragmentShaderSource = new("Shaders/shader.frag");
        PShaderSource fragmentShader = new(ShaderType.FragmentShader, FragmentShaderSource);
        // shader
        _shader = new(_gl, vertexShader, fragmentShader);
        World.ActiveWorld.SystemsManager.Shader = _shader;

        // sprite
        _sprite = new(_gl, PMesh<float, uint>.Quad, _texture, _shader);
        _sprite.Load();

        _template =
            new EntityTemplate(
                new Type[]
                {
                    typeof(SpriteComponent)
                },
                new IPleiadComponent[]
                {
                    new SpriteComponent()
                    {
                        sprite = _sprite
                    }
                });
    }

    public void InputRegistration(ref InputListener listener)
    {
        listener.KeyboardEvents.OnKeyDown += OnKeyDown;
        listener.KeyboardEvents.OnKeyboadrKeyUp += OnKeyUp;
    }

    private void OnKeyDown(IKeyboard keyboard, Key key, int code)
    {
        if (key == Key.Y)
        {
            _shouldAdd = true;
        }
    }

    private void OnKeyUp(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Y)
        {
            _shouldAdd = false;
        }
    }
}


public struct SpriteLoadingTask : IPleiadTaskOn<SpriteComponent>
{
    public void RunOn(int i, ref SpriteComponent[] array)
    {
        if (array[i].sprite is not null)
        {
            array[i].sprite.Load(); 
        }
    }
}
public struct SpriteRenderingTask : IPleiadTaskOn<SpriteComponent>
{
    public void RunOn(int i, ref SpriteComponent[] array)
    {
        if (array[i].sprite is not null)
        {
            array[i].sprite.Draw(); 
        }
    }
}

public class SpriteRenderingSystem : IRenderSystem
{
    private static Vector3 CameraPosition = new Vector3(0.0f, 0.0f, 3.0f);
    private static Vector3 CameraTarget = Vector3.Zero;
    private static Vector3 CameraDirection = Vector3.Normalize(CameraPosition - CameraTarget);
    private static Vector3 CameraRight = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, CameraDirection));
    private static Vector3 CameraUp = Vector3.Cross(CameraDirection, CameraRight);
    private static int Width = World.ActiveWorld.SystemsManager.WindowWidth;
    private static int Height = World.ActiveWorld.SystemsManager.WindowHeight;

    public void Load()
    {
        var _gl = World.ActiveWorld.SystemsManager.Api;

        // texture
        PTexture _texture = new(_gl, new(@"Textures/texture.png"));

        // shaders
        //vertex shader
        FileContract VertexShaderSource = new("Shaders/shader.vert");
        PShaderSource vertexShader = new(ShaderType.VertexShader, VertexShaderSource);
        //fragment shader
        FileContract FragmentShaderSource = new("Shaders/shader.frag");
        PShaderSource fragmentShader = new(ShaderType.FragmentShader, FragmentShaderSource);
        // shader
        PShader _shader = new(_gl, vertexShader, fragmentShader);
        World.ActiveWorld.SystemsManager.Shader = _shader;

        // sprite
        PSprite _sprite = new(_gl, PMesh<float, uint>.Quad, _texture, _shader);
        _sprite.Load();
    }

    public void Render(double obj)
    {
        var shader = World.ActiveWorld.SystemsManager.Shader;
        if (shader is null)
        {
            return;
        }

        TaskManager.SetTask(new TaskOnHandle<SpriteComponent>(new SpriteRenderingTask()), true);


        var view = Matrix4x4.CreateLookAt(CameraPosition, CameraTarget, CameraUp);
        //var projection = Matrix4x4.CreatePerspectiveFieldOfView(PTransform.DegreesToRadians(45.0f), Width / Height, 0.1f, 100.0f);
        var projection = Matrix4x4.CreateOrthographicOffCenter(-1.0f * Width / Height, 1.0f * Width / Height, -1.0f, 1.0f, 0.1f, 100.0f);
        shader.SetUniform("uView", view);
        shader.SetUniform("uProjection", projection);
    }
}



public struct SpriteMovingTask : IPleiadTaskOn<SpriteComponent>
{
    public PTransform Transformation;
    public void RunOn(int i, ref SpriteComponent[] array)
    {
        array[i].sprite.Transform(Transformation);
    }
}
public class SpriteMovingSystem : IPleiadSystem, IRegisterInput
{
    static bool shouldTransform = false;
    static bool isTransformQueued = false;
    static PTransform transform = new();

    public void Cycle(double dTime)
    {
        if (shouldTransform)
        {
            TaskOnHandle<SpriteComponent> handle = new(new SpriteMovingTask()
            {
                Transformation = transform
            });

            TaskManager.SetTask(handle, true);
            shouldTransform = false;
            isTransformQueued = false;
        }
    }

    public void InputRegistration(ref InputListener listener)
    {
        listener.KeyboardEvents.OnKeyDown += OnKeyDown;
        listener.KeyboardEvents.OnKeyboadrKeyUp += OnKeyUp;
    }

    private void OnKeyUp(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.A || key == Key.D)
        {
            if (isTransformQueued) return;

            shouldTransform = false;
        }
    }

    private void OnKeyDown(IKeyboard keyboard, Key key, int code)
    {
        switch (key)
        {
            case Key.A:
                {
                    QueueTransform();
                    transform.Position = new(-0.1f, 0.0f, 0.0f);
                    break;
                }
            case Key.D:
                {
                    QueueTransform();
                    transform.Position = new(0.1f, 0.0f, 0.0f);
                    break;
                }
            case Key.W:
                {
                    QueueTransform();
                    transform.Position = new(0.0f, 0.1f, 0.0f);
                    break;
                }
            case Key.S:
                {
                    QueueTransform();
                    transform.Position = new(0.0f, -0.1f, 0.0f);
                    break;
                }
            case Key.E:
                {
                    QueueTransform();
                    transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, PTransform.DegreesToRadians(22.5f));
                    break;
                }
            case Key.Q:
                {
                    QueueTransform();
                    transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, PTransform.DegreesToRadians(-22.5f));
                    break;
                }
            default:
                break;
        }
    }

    private void QueueTransform()
    {
        shouldTransform = true;
        isTransformQueued = true;
    }
}

