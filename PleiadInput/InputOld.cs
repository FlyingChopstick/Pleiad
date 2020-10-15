namespace PleiadInput
{
    //public static class InputOld
    //{
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


    //    public static void SetupEvents()
    //    {
    //        _keyPressed = new Dictionary<Key, bool>();
    //        _actions = new Dictionary<Key, List<EventPack>>();

    //        _keys = new List<bool>(27);
    //        for (int i = 0; i < 27; i++)
    //        {
    //            _keys.Add(false);
    //        }
    //    }


    //    public static void Update()
    //    {
    //        HandlePress(Key.A);
    //        HandlePress(Key.B);
    //        HandlePress(Key.B);
    //        HandlePress(Key.C);
    //        HandlePress(Key.D);
    //        HandlePress(Key.E);
    //        HandlePress(Key.F);
    //        HandlePress(Key.G);
    //        HandlePress(Key.H);
    //        HandlePress(Key.I);
    //        HandlePress(Key.J);
    //        HandlePress(Key.K);
    //        HandlePress(Key.L);
    //        HandlePress(Key.M);
    //        HandlePress(Key.N);
    //        HandlePress(Key.O);
    //        HandlePress(Key.P);
    //        HandlePress(Key.Q);
    //        HandlePress(Key.R);
    //        HandlePress(Key.S);
    //        HandlePress(Key.T);
    //        HandlePress(Key.U);
    //        HandlePress(Key.V);
    //        HandlePress(Key.W);
    //        HandlePress(Key.X);
    //        HandlePress(Key.Y);
    //        HandlePress(Key.Z);
    //        HandlePress(Key.Z);
    //        HandlePress(Key.Escape);


    //        //ClearConsole();
    //        //_keys[0].HandlePress(Key.A, KeyA_pressed, KeyA_released);
    //        //_keys[1].HandlePress(Key.B, KeyB_pressed, KeyB_released);
    //        //_keys[2].HandlePress(Key.B, KeyB_pressed, KeyB_released);
    //        //_keys[3].HandlePress(Key.C, KeyC_pressed, KeyC_released);
    //        //_keys[4].HandlePress(Key.D, KeyD_pressed, KeyD_released);
    //        //_keys[5].HandlePress(Key.E, KeyE_pressed, KeyE_released);
    //        //_keys[6].HandlePress(Key.F, KeyF_pressed, KeyF_released);
    //        //_keys[7].HandlePress(Key.G, KeyG_pressed, KeyG_released);
    //        //_keys[8].HandlePress(Key.H, KeyH_pressed, KeyH_released);
    //        //_keys[9].HandlePress(Key.I, KeyI_pressed, KeyI_released);
    //        //_keys[10].HandlePress(Key.J, KeyJ_pressed, KeyJ_released);
    //        //_keys[11].HandlePress(Key.K, KeyK_pressed, KeyK_released);
    //        //_keys[12].HandlePress(Key.L, KeyL_pressed, KeyL_released);
    //        //_keys[13].HandlePress(Key.M, KeyM_pressed, KeyM_released);
    //        //_keys[14].HandlePress(Key.N, KeyN_pressed, KeyN_released);
    //        //_keys[15].HandlePress(Key.O, KeyO_pressed, KeyO_released);
    //        //_keys[16].HandlePress(Key.P, KeyP_pressed, KeyP_released);
    //        //_keys[17].HandlePress(Key.Q, KeyQ_pressed, KeyQ_released);
    //        //_keys[18].HandlePress(Key.R, KeyR_pressed, KeyR_released);
    //        //_keys[19].HandlePress(Key.S, KeyS_pressed, KeyS_released);
    //        //_keys[20].HandlePress(Key.T, KeyT_pressed, KeyT_released);
    //        //_keys[21].HandlePress(Key.U, KeyU_pressed, KeyU_released);
    //        //_keys[22].HandlePress(Key.V, KeyV_pressed, KeyV_released);
    //        //_keys[23].HandlePress(Key.W, KeyW_pressed, KeyW_released);
    //        //_keys[24].HandlePress(Key.X, KeyX_pressed, KeyX_released);
    //        //_keys[25].HandlePress(Key.Y, KeyY_pressed, KeyY_released);
    //        //_keys[26].HandlePress(Key.Z, KeyZ_pressed, KeyZ_released);
    //        //_keys[27].HandlePress(Key.Z, KeyZ_pressed, KeyZ_released);
    //        //HandlePress(Key.A, KeyA_pressed, KeyA_released);
    //        //HandlePress(Key.B, KeyB_pressed, KeyB_released);
    //        //HandlePress(Key.B, KeyB_pressed, KeyB_released);
    //        //HandlePress(Key.C, KeyC_pressed, KeyC_released);
    //        //HandlePress(Key.D, KeyD_pressed, KeyD_released);
    //        //HandlePress(Key.E, KeyE_pressed, KeyE_released);
    //        //HandlePress(Key.F, KeyF_pressed, KeyF_released);
    //        //HandlePress(Key.G, KeyG_pressed, KeyG_released);
    //        //HandlePress(Key.H, KeyH_pressed, KeyH_released);
    //        //HandlePress(Key.I, KeyI_pressed, KeyI_released);
    //        //HandlePress(Key.J, KeyJ_pressed, KeyJ_released);
    //        //HandlePress(Key.K, KeyK_pressed, KeyK_released);
    //        //HandlePress(Key.L, KeyL_pressed, KeyL_released);
    //        //HandlePress(Key.M, KeyM_pressed, KeyM_released);
    //        //HandlePress(Key.N, KeyN_pressed, KeyN_released);
    //        //HandlePress(Key.O, KeyO_pressed, KeyO_released);
    //        //HandlePress(Key.P, KeyP_pressed, KeyP_released);
    //        //HandlePress(Key.Q, KeyQ_pressed, KeyQ_released);
    //        //HandlePress(Key.R, KeyR_pressed, KeyR_released);
    //        //HandlePress(Key.S, KeyS_pressed, KeyS_released);
    //        //HandlePress(Key.T, KeyT_pressed, KeyT_released);
    //        //HandlePress(Key.U, KeyU_pressed, KeyU_released);
    //        //HandlePress(Key.V, KeyV_pressed, KeyV_released);
    //        //HandlePress(Key.W, KeyW_pressed, KeyW_released);
    //        //HandlePress(Key.X, KeyX_pressed, KeyX_released);
    //        //HandlePress(Key.Y, KeyY_pressed, KeyY_released);
    //        //HandlePress(Key.Z, KeyZ_pressed, KeyZ_released);
    //        //HandlePress(Key.Z, KeyZ_pressed, KeyZ_released);
    //        //HandlePress(Key.Escape, Escape_pressed, Escape_released);
    //        //A.HandlePress(Key.A, KeyA_pressed, KeyA_released);
    //        //B.HandlePress(Key.B, KeyB_pressed, KeyB_released);
    //        //B.HandlePress(Key.B, KeyB_pressed, KeyB_released);
    //        //C.HandlePress(Key.C, KeyC_pressed, KeyC_released);
    //        //D.HandlePress(Key.D, KeyD_pressed, KeyD_released);
    //        //E.HandlePress(Key.E, KeyE_pressed, KeyE_released);
    //        //F.HandlePress(Key.F, KeyF_pressed, KeyF_released);
    //        //G.HandlePress(Key.G, KeyG_pressed, KeyG_released);
    //        //H.HandlePress(Key.H, KeyH_pressed, KeyH_released);
    //        //I.HandlePress(Key.I, KeyI_pressed, KeyI_released);
    //        //J.HandlePress(Key.J, KeyJ_pressed, KeyJ_released);
    //        //K.HandlePress(Key.K, KeyK_pressed, KeyK_released);
    //        //L.HandlePress(Key.L, KeyL_pressed, KeyL_released);
    //        //M.HandlePress(Key.M, KeyM_pressed, KeyM_released);
    //        //N.HandlePress(Key.N, KeyN_pressed, KeyN_released);
    //        //O.HandlePress(Key.O, KeyO_pressed, KeyO_released);
    //        //P.HandlePress(Key.P, KeyP_pressed, KeyP_released);
    //        //Q.HandlePress(Key.Q, KeyQ_pressed, KeyQ_released);
    //        //R.HandlePress(Key.R, KeyR_pressed, KeyR_released);
    //        //S.HandlePress(Key.S, KeyS_pressed, KeyS_released);
    //        //T.HandlePress(Key.T, KeyT_pressed, KeyT_released);
    //        //U.HandlePress(Key.U, KeyU_pressed, KeyU_released);
    //        //V.HandlePress(Key.V, KeyV_pressed, KeyV_released);
    //        //W.HandlePress(Key.W, KeyW_pressed, KeyW_released);
    //        //X.HandlePress(Key.X, KeyX_pressed, KeyX_released);
    //        //Y.HandlePress(Key.Y, KeyY_pressed, KeyY_released);
    //        //Z.HandlePress(Key.Z, KeyZ_pressed, KeyZ_released);
    //        //Escape.HandlePress(Key.Escape, Escape_pressed, Escape_released);
    //    }


    //    public delegate void KeyPress(Key key);
    //    public delegate void KeyRelease(Key key);

    //    private static Dictionary<Key, bool> _keyPressed;
    //    //private static Dictionary<Key, EventPack> _keyRegistry;

    //    private static List<bool> _keys;
    //    //private static Dictionary<Key, List<EventPack>> _actions;


    //    //public static void HandlePress(Key key, KeyPress p, KeyRelease r)
    //    //{
    //    //    if (Keyboard.IsKeyDown(key))
    //    //    {
    //    //        _keyStates[key] = true;
    //    //        p?.Invoke(key);

    //    //        try
    //    //        {
    //    //            foreach (var action in _actions[key])
    //    //            {
    //    //                action.Press?.Invoke();
    //    //            }
    //    //        }
    //    //        catch (KeyNotFoundException) { }
    //    //        //if (!_keyStates[key])
    //    //        //{

    //    //        //}
    //    //    }
    //    //    else
    //    //    {
    //    //        if (_keyStates[key])
    //    //        {
    //    //            _keyStates[key] = false;
    //    //            r?.Invoke(key);

    //    //            try
    //    //            {

    //    //                foreach (var action in _actions[key])
    //    //                {
    //    //                    action.Release?.Invoke();
    //    //                }
    //    //            }
    //    //            catch (KeyNotFoundException) { }
    //    //        }
    //    //    }

    //    //    //if (Keyboard.IsKeyDown(key))
    //    //    //{
    //    //    //    if (!current)
    //    //    //    {
    //    //    //        current = true;
    //    //    //        p?.Invoke(key);
    //    //    //    }
    //    //    //}
    //    //    //else
    //    //    //{
    //    //    //    if (current)
    //    //    //    {
    //    //    //        current = false;
    //    //    //        r?.Invoke(key);
    //    //    //    }
    //    //    //}
    //    //}
    //    //public static void HandlePress(Key key)
    //    //{
    //    //    if (Keyboard.IsKeyDown(key))
    //    //    {
    //    //        try
    //    //        {
    //    //            foreach (var action in _actions[key])
    //    //            {
    //    //                action.Press?.Invoke();
    //    //            }
    //    //        }
    //    //        catch (KeyNotFoundException) { }
    //    //    }
    //    //    else
    //    //    {
    //    //        if (_keyStates[key])
    //    //        {
    //    //            try
    //    //            {
    //    //                foreach (var action in _actions[key])
    //    //                {
    //    //                    action.Release?.Invoke();
    //    //                }
    //    //            }
    //    //            catch (KeyNotFoundException) { }
    //    //        }
    //    //    }
    //    //}
    //    public static bool HandlePress(Key key)
    //    {
    //        if (Keyboard.IsKeyDown(key))
    //        {
    //            try
    //            {
    //                _keyPressed[key] = true;
    //                foreach (var action in _actions[key])
    //                {
    //                    action.Press?.Invoke();
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
    //                    foreach (var action in _actions[key])
    //                    {
    //                        action.Release?.Invoke();
    //                    }
    //                }
    //            }
    //            catch (KeyNotFoundException) { }
    //            return false;
    //        }
    //    }


    //    //public static void Assign(this Key key, EventPack pack)
    //    //{
    //    //    if (!_actions.ContainsKey(key))
    //    //        _actions[key] = new List<EventPack>();

    //    //    _actions[key].Add(pack);
    //    //}

    //    //#region Press Events
    //    //public static event KeyPress KeyA_pressed;
    //    //public static event KeyPress KeyB_pressed;
    //    //public static event KeyPress KeyC_pressed;
    //    //public static event KeyPress KeyD_pressed;
    //    //public static event KeyPress KeyE_pressed;
    //    //public static event KeyPress KeyF_pressed;
    //    //public static event KeyPress KeyG_pressed;
    //    //public static event KeyPress KeyH_pressed;
    //    //public static event KeyPress KeyI_pressed;
    //    //public static event KeyPress KeyJ_pressed;
    //    //public static event KeyPress KeyK_pressed;
    //    //public static event KeyPress KeyL_pressed;
    //    //public static event KeyPress KeyM_pressed;
    //    //public static event KeyPress KeyN_pressed;
    //    //public static event KeyPress KeyO_pressed;
    //    //public static event KeyPress KeyP_pressed;
    //    //public static event KeyPress KeyQ_pressed;
    //    //public static event KeyPress KeyR_pressed;
    //    //public static event KeyPress KeyS_pressed;
    //    //public static event KeyPress KeyT_pressed;
    //    //public static event KeyPress KeyU_pressed;
    //    //public static event KeyPress KeyV_pressed;
    //    //public static event KeyPress KeyW_pressed;
    //    //public static event KeyPress KeyX_pressed;
    //    //public static event KeyPress KeyY_pressed;
    //    //public static event KeyPress KeyZ_pressed;
    //    //public static event KeyPress Escape_pressed;
    //    //#endregion
    //    //#region Release events
    //    //public static event KeyRelease KeyA_released;
    //    //public static event KeyRelease KeyB_released;
    //    //public static event KeyRelease KeyC_released;
    //    //public static event KeyRelease KeyD_released;
    //    //public static event KeyRelease KeyE_released;
    //    //public static event KeyRelease KeyF_released;
    //    //public static event KeyRelease KeyG_released;
    //    //public static event KeyRelease KeyH_released;
    //    //public static event KeyRelease KeyI_released;
    //    //public static event KeyRelease KeyJ_released;
    //    //public static event KeyRelease KeyK_released;
    //    //public static event KeyRelease KeyL_released;
    //    //public static event KeyRelease KeyM_released;
    //    //public static event KeyRelease KeyN_released;
    //    //public static event KeyRelease KeyO_released;
    //    //public static event KeyRelease KeyP_released;
    //    //public static event KeyRelease KeyQ_released;
    //    //public static event KeyRelease KeyR_released;
    //    //public static event KeyRelease KeyS_released;
    //    //public static event KeyRelease KeyT_released;
    //    //public static event KeyRelease KeyU_released;
    //    //public static event KeyRelease KeyV_released;
    //    //public static event KeyRelease KeyW_released;
    //    //public static event KeyRelease KeyX_released;
    //    //public static event KeyRelease KeyY_released;
    //    //public static event KeyRelease KeyZ_released;
    //    //public static event KeyRelease Escape_released;
    //    //#endregion

    //    //#region States

    //    //private static bool A;
    //    //private static bool B;
    //    //private static bool C;
    //    //private static bool D;
    //    //private static bool E;
    //    //private static bool F;
    //    //private static bool G;
    //    //private static bool H;
    //    //private static bool I;
    //    //private static bool J;
    //    //private static bool K;
    //    //private static bool L;
    //    //private static bool M;
    //    //private static bool N;
    //    //private static bool O;
    //    //private static bool P;
    //    //private static bool Q;
    //    //private static bool R;
    //    //private static bool S;
    //    //private static bool T;
    //    //private static bool U;
    //    //private static bool V;
    //    //private static bool W;
    //    //private static bool X;
    //    //private static bool Y;
    //    //private static bool Z;
    //    //private static bool Escape;
    //    //#endregion





    //    //public static void HandlePress(this bool current, Key key, KeyPress p)
    //    //{
    //    //    if (Keyboard.IsKeyDown(key))
    //    //    {
    //    //        if (!current)
    //    //        {
    //    //            current = true;
    //    //            p?.Invoke(key);
    //    //        }
    //    //    }
    //    //}
    //    //public static void HandlePress(this bool current, Key key)
    //    //{
    //    //    if (Keyboard.IsKeyDown(key))
    //    //    {
    //    //        if (!current)
    //    //        {
    //    //            current = true;
    //    //            _keyRegistry[key].Press?.Invoke(key);
    //    //        }
    //    //    }
    //    //    else
    //    //    {
    //    //        if (current)
    //    //        {
    //    //            current = false;
    //    //            _keyRegistry[key].Release?.Invoke(key);
    //    //        }
    //    //    }
    //    //}

    //}
}
