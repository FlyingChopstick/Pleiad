using Pleiad.Render;
using Pleiad.Render.ControlEvents;

namespace Pleiad.Input
{
    public class InputListener
    {
        //public static void ClearConsole()
        //{
        //    Console.Clear();
        //    while (Console.KeyAvailable) Console.ReadKey(false);
        //    //Console.ReadKey();
        //    Console.Clear();
        //}

        //public event KeyEvent KeyRelease;
        //public event KeyEvent KeyPress;


        //private List<Key> _activeList;
        //private readonly List<Key> _allKeys;
        //private readonly List<Key> _inputTable;
        //private readonly Dictionary<Key, bool> _keyState;

        //private bool _useInputTable = true;
        //public bool UseInputTable
        //{
        //    get => _useInputTable;
        //    set
        //    {
        //        if (value)
        //        {
        //            Console.WriteLine("Listening to Input Table");
        //            _activeList = _inputTable;
        //        }
        //        else
        //        {
        //            Console.WriteLine("Listening to all keys");
        //            _activeList = _allKeys;
        //        }
        //        _useInputTable = value;
        //    }
        //}

        public InputListener(bool useInputTable = true)
        {
            //_allKeys = Enum.GetValues(typeof(Key)).Cast<Key>().ToList();
            //_keyState = new Dictionary<Key, bool>();

            //_inputTable = new List<Key>();
            //UseInputTable = useInputTable;

            //UseInputTable = useInputTable;
        }


        PWindow _activeWindow;
        public KeyboardEvents KeyboardEvents { get; private set; }
        public MouseEvents MouseEvents { get; private set; }
        public GamepadEvents GamepadEvents { get; private set; }

        public void AttachToWindow(PWindow window)
        {
            _activeWindow = window;

            KeyboardEvents = _activeWindow.KeyboardEvents;
            MouseEvents = _activeWindow.MouseEvents;
            GamepadEvents = _activeWindow.GamepadEvents;
        }

        //public void ListenTo(Key key)
        //{
        //    if (!_inputTable.Contains(key))
        //        _inputTable.Add(key);
        //}
        //public void ListenTo(Key[] keys)
        //{
        //    foreach (var key in keys)
        //    {
        //        if (!_inputTable.Contains(key))
        //            _inputTable.Add(key);
        //    }
        //}
        //public void StopListening(Key key)
        //{
        //    if (_inputTable.Contains(key))
        //        _inputTable.Remove(key);
        //}

        //public void ReadKeys()
        //{
        //    foreach (var key in _activeList)
        //    {
        //        CheckKey(key);
        //    }
        //}
        //public void WaitForInput(Key[] keys)
        //{
        //    bool waiting = true;
        //    do
        //    {
        //        foreach (var key in keys)
        //        {
        //            if (!_keyState.ContainsKey(key)) _keyState[key] = false;
        //            CheckKey(key);
        //            if (_keyState[key])
        //            {
        //                waiting = false;
        //                break;
        //            }
        //        }
        //    } while (waiting);
        //}
        //public void WaitForInput(List<Key> keys)
        //{
        //    bool waiting = true;
        //    do
        //    {
        //        foreach (var key in keys)
        //        {
        //            if (!_keyState.ContainsKey(key)) _keyState[key] = false;
        //            CheckKey(key);

        //            if (_keyState[key])
        //            {
        //                waiting = false;
        //                break;
        //            }
        //        }
        //    } while (waiting);
        //}
        //public void WaitForInput(Key key)
        //{
        //    if (!_keyState.ContainsKey(key)) _keyState[key] = false;

        //    while (!_keyState[key])
        //    {
        //        CheckKey(key);
        //    }
        //}


        //private void CheckKey(Key key)
        //{
        //    short state = GetAsyncKeyState((short)key);
        //    if (state == short.MinValue)
        //    {
        //        KeyPress?.Invoke(key);
        //        _keyState[key] = true;
        //    }
        //    else
        //    {
        //        if (!_keyState.ContainsKey(key))
        //        {
        //            _keyState[key] = false;
        //        }
        //        if (_keyState[key])
        //        {
        //            KeyRelease?.Invoke(key);
        //            _keyState[key] = false;
        //        }
        //    }
        //}


        //[DllImport("user32.dll")]
        //internal static extern short GetAsyncKeyState(short vKey);
    }
}
