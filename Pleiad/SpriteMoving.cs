using System;
using System.Numerics;
using Pleiad.Entities;
using Pleiad.Entities.Components;
using Pleiad.Input;
using Pleiad.Render;
using Pleiad.Render.Models;
using Pleiad.Systems.Interfaces;
using Pleiad.Tasks;
using Pleiad.Tasks.Interfaces;
using Pleiad.Worlds;
using Silk.NET.Input;
using Silk.NET.OpenGL;

public class SpriteEntityAdditionSystem : IPleiadSystem, IRegisterInput
{
    static bool _shouldAdd = false;

    static GL _gl;
    static PTexture _texture;
    static PSprite _sprite;
    static EntityTemplate _template;

    public void Cycle(double dTime)
    {
        if (_shouldAdd)
        {
            LoadTemplate();

            World.ActiveWorld.EntityManager.AddEntity(_template);
            _shouldAdd = false;
        }
    }
    private static void LoadTemplate()
    {
        _gl = PleiadRenderer.Api;

        // texture
        _texture = new(_gl, new(@"Textures/texture.png"));
        // sprite
        _sprite = new(_gl, PMesh<float, uint>.Quad, _texture);
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
        listener.KeyboardEvents.OnKeyboadrKeyUp += OnKeyUp;
    }
    private void OnKeyUp(IKeyboard keyboard, Key key, int keyCode)
    {
        if (key == Key.Y)
        {
            _shouldAdd = true;
        }
    }
}


public struct SpriteRenderingTask : IPleiadRenderTask<SpriteComponent>
{
    public void Draw(int index, ref SpriteComponent[] array)
    {
        PleiadRenderer.DrawSprite(array[index].sprite);
    }
}

public class SpriteRenderingSystem : IRenderSystem
{
    public void Render(double obj)
    {
        TaskManager.SetRenderTask(new RenderTaskHandle<SpriteComponent>(new SpriteRenderingTask()));
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
        }
    }
    public void InputRegistration(ref InputListener listener)
    {
        listener.KeyboardEvents.OnKeyDown += OnKeyDown;
    }
    private void OnKeyDown(IKeyboard keyboard, Key key, int code)
    {
        transform = new();
        switch (key)
        {
            case Key.A:
                {
                    shouldTransform = true;
                    transform.Position = new(-0.1f, 0.0f, 0.0f);
                    break;
                }
            case Key.D:
                {
                    shouldTransform = true;
                    transform.Position = new(0.1f, 0.0f, 0.0f);
                    break;
                }
            case Key.W:
                {
                    shouldTransform = true;
                    transform.Position = new(0.0f, 0.1f, 0.0f);
                    break;
                }
            case Key.S:
                {
                    shouldTransform = true;
                    transform.Position = new(0.0f, -0.1f, 0.0f);
                    break;
                }
            case Key.E:
                {
                    shouldTransform = true;
                    transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, PTransform.DegreesToRadians(22.5f));
                    break;
                }
            case Key.Q:
                {
                    shouldTransform = true;
                    transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, PTransform.DegreesToRadians(-22.5f));
                    break;
                }
            default:
                break;
        }
    }
}