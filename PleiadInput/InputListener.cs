using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace PleiadInput
{
    public class InputListener
    {
        public static void ClearConsole()
        {
            Console.Clear();
            while (Console.KeyAvailable) Console.ReadKey(false);
            //Console.ReadKey();
            Console.Clear();
        }


        public event KeyEvent KeyRelease;
        public event KeyEvent KeyPress;

        private List<Key> _keys;
        private Dictionary<Key, bool> _keyState;

        private List<Key> _inputTable;
        public bool UseInputTable { get; set; }

        public InputListener(bool useInputTable)
        {
            _keys = Enum.GetValues(typeof(Key)).Cast<Key>().ToList();
            _keyState = new Dictionary<Key, bool>();

            UseInputTable = useInputTable;
            _inputTable = new List<Key>();
        }

        public void ListenTo(Key key)
        {
            if (!_inputTable.Contains(key))
                _inputTable.Add(key);
        }
        public void ListenTo(Key[] keys)
        {
            foreach (var key in keys)
            {
                if (!_inputTable.Contains(key))
                    _inputTable.Add(key);
            }
        }
        public void StopListening(Key key)
        {
            if (_inputTable.Contains(key))
                _inputTable.Remove(key);
        }


        public void ReadKeys()
        {
            if (!UseInputTable)
            {
                foreach (var key in _keys)
                {
                    CheckKey(key);
                }
            }
            else
            {
                foreach (var key in _inputTable)
                {
                    CheckKey(key);
                }
            }
        }
        public void WaitForInput(Key[] keys)
        {
            bool waiting = true;
            do
            {
                foreach (var key in keys)
                {
                    CheckKey(key);
                    if (_keyState.ContainsKey(key) && _keyState[key])
                    {
                        waiting = false;
                        break;
                    }
                }
            } while (waiting);
        }
        public void WaitForInput(List<Key> keys)
        {
            bool waiting = true;
            do
            {
                foreach (var key in keys)
                {
                    CheckKey(key);
                    if (_keyState.ContainsKey(key) && _keyState[key])
                    {
                        waiting = false;
                        break;
                    }
                }
            } while (waiting);
        }
        public void WaitForInput(Key key)
        {
            while (!_keyState[key])
            {
                CheckKey(key);
            }
        }


        private void CheckKey(Key key)
        {
            short state = GetAsyncKeyState((short)key);
            var bit = state & 0x8000;
            if (state == short.MinValue)
            {
                KeyPress?.Invoke(key);
                _keyState[key] = true;
            }
            else
            {
                if (_keyState.ContainsKey(key) && _keyState[key])
                {
                    KeyRelease?.Invoke(key);
                    _keyState[key] = false;
                }
            }
        }


        [DllImport("user32.dll")]
        internal static extern short GetAsyncKeyState(short vKey);
    }
}
