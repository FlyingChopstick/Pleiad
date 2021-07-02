using Silk.NET.Input;

namespace Pleiad.Render.ControlEvents
{
    public class KeyboardEvents
    {
        internal void Keyboard_KeyChar(IKeyboard keyboard, char character)
        {
            OnKeyboardKeyChar?.Invoke(keyboard, character);
        }
        public delegate void OnKeyboardKeyCharHandler(IKeyboard keyboard, char character);
        public event OnKeyboardKeyCharHandler OnKeyboardKeyChar;

        internal void Keyboard_KeyUp(IKeyboard keyboard, Key key, int keyCode)
        {
            OnKeyboadrKeyUp?.Invoke(keyboard, key, keyCode);
        }
        public delegate void OnKeyboadrKeyUpHandler(IKeyboard keyboard, Key key, int keyCode);
        public event OnKeyboadrKeyUpHandler OnKeyboadrKeyUp;

        internal void Keyboard_KeyDown(IKeyboard keyboard, Key key, int keyCode)
        {
            OnKeyDown?.Invoke(keyboard, key, keyCode);
        }
        public delegate void OnKeyDownHandler(IKeyboard keyboard, Key key, int code);
        public event OnKeyDownHandler OnKeyDown;
    }
}
