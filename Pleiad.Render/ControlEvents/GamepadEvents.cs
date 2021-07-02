using Silk.NET.Input;

namespace Pleiad.Render.ControlEvents
{
    public class GamepadEvents
    {
        internal void Gamepad_TriggerMoved(IGamepad gamepad, Trigger trigger)
        {
            OnGamepadTriggerMove?.Invoke(gamepad, trigger);
        }
        public delegate void OnGamepadTriggerMoveHandler(IGamepad gamepad, Trigger trigger);
        public event OnGamepadTriggerMoveHandler OnGamepadTriggerMove;

        internal void Gamepad_ThumbstickMoved(IGamepad gamepad, Thumbstick thumbstick)
        {
            OnGamepadThumbstickMoved?.Invoke(gamepad, thumbstick);
        }
        public delegate void OnGamepadThumbstickMovedHandler(IGamepad gamepad, Thumbstick trigger);
        public event OnGamepadThumbstickMovedHandler OnGamepadThumbstickMoved;

        internal void Gamepad_ButtonUp(IGamepad gamepad, Button button)
        {
            OnGamepadButtonUp?.Invoke(gamepad, button);
        }
        public delegate void OnGamepadButtonUpHandler(IGamepad gamepad, Button button);
        public event OnGamepadButtonUpHandler OnGamepadButtonUp;

        internal void Gamepad_ButtonDown(IGamepad gamepad, Button button)
        {
            OnGamepadButtonDown?.Invoke(gamepad, button);
        }
        public delegate void OnGamepadButtonDownHandler(IGamepad gamepad, Button button);
        public event OnGamepadButtonDownHandler OnGamepadButtonDown;
    }
}
