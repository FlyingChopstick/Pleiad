﻿using System;
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


        private List<Key> _activeList;
        private readonly List<Key> _allKeys;
        private readonly List<Key> _inputTable;
        private readonly Dictionary<Key, bool> _keyState;

        private bool _useInputTable = true;
        public bool UseInputTable
        {
            get => _useInputTable;
            set
            {
                if (value)
                {
                    Console.WriteLine("Listening to Input Table");
                    _activeList = _inputTable;
                }
                else
                {
                    Console.WriteLine("Listening to all keys");
                    _activeList = _allKeys;
                }
                _useInputTable = value;
            }
        }

        public InputListener(bool useInputTable = true)
        {
            _allKeys = Enum.GetValues(typeof(Key)).Cast<Key>().ToList();
            _keyState = new Dictionary<Key, bool>();

            UseInputTable = useInputTable;
            _inputTable = new List<Key>();

            UseInputTable = useInputTable;
        }

        /// <summary>
        /// Adds a new key to listen for
        /// </summary>
        /// <param name="key">Key to listen</param>
        public void ListenTo(Key key)
        {
            if (!_inputTable.Contains(key))
                _inputTable.Add(key);
        }
        /// <summary>
        /// Adds a list of keys to listen for
        /// </summary>
        /// <param name="keys">List of keys</param>
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

        /// <summary>
        /// Checks for keypresses for registered keys
        /// </summary>
        public void ReadKeys()
        {
            foreach (var key in _activeList)
            {
                CheckKey(key);
            }
        }
        /// <summary>
        /// Waits for a key press
        /// </summary>
        /// <param name="keys">Keys to check</param>
        public void WaitForInput(Key[] keys)
        {
            bool waiting = true;
            do
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    if (!_keyState.ContainsKey(keys[i])) _keyState[keys[i]] = false;
                    CheckKey(keys[i]);
                    if (_keyState[keys[i]])
                    {
                        waiting = false;
                        break;
                    }
                }
            } while (waiting);
        }
        /// <summary>
        /// Waits for a key press
        /// </summary>
        /// <param name="keys">Keys to check</param>
        public void WaitForInput(List<Key> keys)
        {
            WaitForInput(keys.ToArray());
        }
        /// <summary>
        /// Waits for a key press
        /// </summary>
        /// <param name="key">Key to check</param>
        public void WaitForInput(Key key)
        {
            if (!_keyState.ContainsKey(key)) _keyState[key] = false;
            while (!_keyState[key])
            {
                CheckKey(key);
            }
        }

        private void CheckKey(Key key)
        {
            short state = GetAsyncKeyState((short)key);
            if (state == short.MinValue)
            {
                KeyPress?.Invoke(key);
                _keyState[key] = true;
            }
            else
            {
                if (!_keyState.ContainsKey(key))
                {
                    _keyState[key] = false;
                }
                if (_keyState[key])
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
