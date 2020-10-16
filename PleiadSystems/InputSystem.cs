namespace PleiadSystems
{
    //public enum EventType
    //{
    //    Press = 0,
    //    Release = 1
    //}

    //public class InputSystem
    //{
    //    private static Dictionary<Key, bool> _keyPressed;

    //    private static List<bool> _keys;
    //    //private static Dictionary<Key, List<EventPack>> _actions;
    //    private static Dictionary<Key, List<Action>> _pressActions;
    //    private static Dictionary<Key, List<Action>> _releaseActions;


    //    public InputSystem()
    //    {
    //        _keyPressed = new Dictionary<Key, bool>();
    //        //_actions = new Dictionary<Key, List<EventPack>>();
    //        _pressActions = new Dictionary<Key, List<Action>>();
    //        _releaseActions = new Dictionary<Key, List<Action>>();


    //        _keys = new List<bool>(27);
    //        for (int i = 0; i < 27; i++)
    //        {
    //            _keys.Add(false);
    //        }

    //        Console.Clear();
    //    }

    //    public void Cycle(float dTime)
    //    {
    //        //HandlePress(Key.A);
    //        //HandlePress(Key.B);
    //        //HandlePress(Key.B);
    //        //HandlePress(Key.C);
    //        //HandlePress(Key.D);
    //        //HandlePress(Key.E);
    //        //HandlePress(Key.F);
    //        //HandlePress(Key.G);
    //        //HandlePress(Key.H);
    //        //HandlePress(Key.I);
    //        //HandlePress(Key.J);
    //        //HandlePress(Key.K);
    //        //HandlePress(Key.L);
    //        //HandlePress(Key.M);
    //        //HandlePress(Key.N);
    //        //HandlePress(Key.O);
    //        //HandlePress(Key.P);
    //        //HandlePress(Key.Q);
    //        //HandlePress(Key.R);
    //        //HandlePress(Key.S);
    //        //HandlePress(Key.T);
    //        //HandlePress(Key.U);
    //        //HandlePress(Key.V);
    //        //HandlePress(Key.W);
    //        //HandlePress(Key.X);
    //        //HandlePress(Key.Y);
    //        //HandlePress(Key.Z);
    //        //HandlePress(Key.Z);
    //        //HandlePress(Key.Escape);
    //    }


    //    public static void ClearConsole()
    //    {
    //        Console.Clear();
    //        while (Console.KeyAvailable) Console.ReadKey(false);
    //        //Console.ReadKey();
    //        Console.Clear();
    //    }
    //    public static void WaitForInput(Key[] keys)
    //    {
    //        bool pressed = false;
    //        while (!pressed)
    //        {
    //            for (int i = 0; i < keys.Length; i++)
    //            {
    //                pressed = HandlePress(keys[i]);
    //                if (pressed)
    //                    break;
    //            }
    //        }
    //    }
    //    public static void WaitForInput(Key key)
    //    {
    //        while (!HandlePress(key)) { }
    //    }
    //    public static bool HandlePress(Key key)
    //    {
    //        if (Keyboard.IsKeyDown(key))
    //        {
    //            _keyPressed[key] = true;
    //            try
    //            {
    //                if (_pressActions[key] != null)
    //                {
    //                    foreach (var action in _pressActions[key])
    //                    {
    //                        action.Invoke();
    //                    }
    //                }
    //                return true;
    //            }
    //            catch (KeyNotFoundException) { return false; }
    //        }
    //        else
    //        {

    //            try
    //            {
    //                if (_keyPressed[key])
    //                {
    //                    _keyPressed[key] = false;
    //                    foreach (var action in _releaseActions[key])
    //                    {
    //                        action.Invoke();
    //                    }
    //                }
    //            }
    //            catch (KeyNotFoundException) { }
    //            return false;
    //        }
    //    }



    //    public static void Assign(Key key, EventType type, Action action)
    //    {
    //        switch (type)
    //        {
    //            case EventType.Press:
    //                {
    //                    if (!_pressActions.ContainsKey(key)) _pressActions[key] = new List<Action>();

    //                    if (!_pressActions[key].Contains(action))
    //                    {
    //                        _pressActions[key].Add(action);
    //                    }
    //                    break;
    //                }
    //            case EventType.Release:
    //                {
    //                    if (!_releaseActions.ContainsKey(key)) _releaseActions[key] = new List<Action>();

    //                    if (!_releaseActions[key].Contains(action))
    //                    {
    //                        _releaseActions[key].Add(action);
    //                    }
    //                    break;
    //                }
    //        }
    //    }
    //}
}
