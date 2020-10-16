namespace PleiadInput
{
    ///// <summary>
    ///// defines the callback type for the hook
    ///// </summary>
    //public delegate int KeyboardHookProc(int code, int wParam, ref KeyboardHookStruct lParam);
    //public struct KeyboardHookStruct
    //{
    //    public int vkCode;
    //    public int scanCode;
    //    public int flags;
    //    public int time;
    //    public int dwExtraInfo;
    //}

    public delegate void KeyEvent(Key key);
    public struct KeyEventData
    {
        public KeyEventData(Key key)
        {
            KeyCode = key;
            Handled = false;
        }

        public Key KeyCode { get; }
        public bool Handled { get; set; }
    }
}
