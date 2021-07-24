using System.Numerics;
using Pleiad.Entities.Components;
using Pleiad.Input;
using Pleiad.Render;
using Pleiad.Render.Camera;
using Pleiad.Systems.Interfaces;
using Pleiad.Tasks;
using Pleiad.Tasks.Interfaces;
using Silk.NET.Input;

namespace Pleiad
{
    enum Direction
    {
        None = 0,
        Right = 1,
        Left = 2,
        Forward = 3,
        Backward = 4,
    }
    struct CameraManipulationTask : IPleiadRenderTask<CameraComponent>
    {
        public float dTime;
        public Direction direction;

        public void Draw(int i, ref CameraComponent[] array)
        {
            var moveSpeed = 5f * dTime;
            var front = PleiadRenderer.Camera.Front;
            var up = PleiadRenderer.Camera.Up;
            Vector3 newPosition = direction switch
            {
                Direction.Right => PleiadRenderer.Camera.Position + Vector3.Normalize(Vector3.Cross(front, up)) * moveSpeed,
                Direction.Left => PleiadRenderer.Camera.Position - Vector3.Normalize(Vector3.Cross(front, up)) * moveSpeed,
                Direction.Forward => PleiadRenderer.Camera.Position + moveSpeed * front,
                Direction.Backward => PleiadRenderer.Camera.Position - moveSpeed * front,
                _ => PleiadRenderer.Camera.Position
            };

            PleiadRenderer.Camera = new PCamera(
                position: newPosition,
                target: newPosition + PleiadRenderer.Camera.Front,
                front: PleiadRenderer.Camera.Front);
        }
    }

    class CameraManipulation : IPleiadSystem, IRegisterInput
    {

        static bool shouldTransform = false;
        static Direction direction = Direction.None;

        public void Cycle(double dTime)
        {
            if (shouldTransform)
            {
                TaskManager.SetRenderTask(new RenderTaskHandle<CameraComponent>(
                    new CameraManipulationTask() { dTime = (float)dTime, direction = direction }));
            }
        }

        public void InputRegistration(ref InputListener listener)
        {
            listener.KeyboardEvents.OnKeyboadrKeyUp += OnKeyUp;
            listener.KeyboardEvents.OnKeyDown += OnKeyDown;
        }

        private void OnKeyDown(IKeyboard keyboard, Key key, int code)
        {
            switch (key)
            {
                case Key.Right:
                    {
                        shouldTransform = true;
                        direction = Direction.Right;
                        break;
                    }
                case Key.Left:
                    {
                        shouldTransform = true;
                        direction = Direction.Left;
                        break;
                    }
                case Key.Up:
                    {
                        shouldTransform = true;
                        direction = Direction.Forward;
                        break;
                    }
                case Key.Down:
                    {
                        shouldTransform = true;
                        direction = Direction.Backward;
                        break;
                    }
                default:
                    break;
            }
        }

        private void OnKeyUp(IKeyboard keyboard, Key key, int keyCode)
        {
            shouldTransform = false;
        }
    }
}
