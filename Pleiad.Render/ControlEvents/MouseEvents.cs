using System.Numerics;
using Silk.NET.Input;

namespace Pleiad.Render.ControlEvents
{
    public class MouseEvents
    {
        internal void Mouse_MouseUp(IMouse mouse, MouseButton button)
        {
            OnMouseUp?.Invoke(mouse, button);
        }
        public delegate void OnMouseUpHandler(IMouse mouse, MouseButton button);
        public event OnMouseUpHandler OnMouseUp;


        internal void Mouse_MouseDown(IMouse mouse, MouseButton button)
        {
            OnMouseDown?.Invoke(mouse, button);
        }
        public delegate void OnMouseDownHandler(IMouse mouse, MouseButton button);
        public event OnMouseDownHandler OnMouseDown;

        internal void Mouse_Click(IMouse mouse, MouseButton button, Vector2 mousePosition)
        {
            OnMouseClick?.Invoke(mouse, button, mousePosition);
        }
        public delegate void OnMouseClickHandler(IMouse mouse, MouseButton button, Vector2 mousePosition);
        public event OnMouseClickHandler OnMouseClick;

        internal void Mouse_DoubleClick(IMouse mouse, MouseButton button, Vector2 mousePosition)
        {
            OnMouseDoubleClick?.Invoke(mouse, button, mousePosition);
        }
        public delegate void OnMouseDoubleClickHandler(IMouse mouse, MouseButton button, Vector2 mousePosition);
        public event OnMouseDoubleClickHandler OnMouseDoubleClick;

        internal void Mouse_Scroll(IMouse mouse, ScrollWheel scrollWheel)
        {
            OnMouseScroll?.Invoke(mouse, scrollWheel);
        }
        public delegate void OnMouseScrollHandler(IMouse mouse, ScrollWheel scroll);
        public event OnMouseScrollHandler OnMouseScroll;

        internal void Mouse_MouseMove(IMouse mouse, Vector2 mousePosition)
        {
            OnMouseMove?.Invoke(mouse, mousePosition);
        }
        public delegate void OnMouseMoveHandler(IMouse mouse, Vector2 move);
        public event OnMouseMoveHandler OnMouseMove;
    }
}
