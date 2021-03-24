using PleiadEntities;
using PleiadExtensions.Files;
using PleiadInput;
using PleiadSystems;
using PleiadTasks;
using PleiadWorld;
using System;

namespace Pleiad
{
    public struct StringTestComponent : IPleiadComponent
    {
        public string text;
    }
    public struct IntTestComponent : IPleiadComponent
    {
        public int testValue;
    }
    public struct FloatTestComponent : IPleiadComponent
    {
        public float testValue;
    }



    public struct TestTask : IPleiadTask
    {
        public float dTime;
        public int num;
        public float value;

        public void Run()
        {
            Console.WriteLine(num);
            value += dTime;
        }
    }

    public struct ConsoleWriteTask : IPleiadTask
    {
        public string message;
        public void Run()
        {
            Console.WriteLine(message);
        }
    }

    public struct ReadAllStrings : IPleiadTaskOn<StringTestComponent>
    {
        public void RunOn(int i, ref StringTestComponent[] array)
        {
            if (array[i].text != null)
            {
                Console.WriteLine(array[i].text);
                array[i] = new StringTestComponent { text = $"Modified {array[i].text}" };
            }
        }
    }

    public struct TestTaskOn<T> : IPleiadTaskOn<StringTestComponent>
    {
        public void RunOn(int i, ref StringTestComponent[] array)
        {
            var c = array[i].text;
            array[i] = new StringTestComponent { text = $"goodbye {c}" };
        }
    }

    public struct TextWriteTask : IPleiadTask
    {
        public FileContract file;
        public string message;
        public void Run()
        {
            file.Write(message);
        }
    }





    public class TestSystem : IPleiadSystem
    {
        public void Cycle(float dTime)
        {
            Console.WriteLine("Test");
        }
    }
    public class WindowCloseSystem : IPleiadSystem, IRegisterInput
    {
        public void Cycle(float dTime)
        {

        }

        public void InputRegistration(ref InputListener listener)
        {
            listener.ListenTo(Key.Escape);
            listener.KeyPress += Listener_KeyPress;
        }

        private void Listener_KeyPress(Key key)
        {
            if (key == Key.Escape)
            {
                World.DefaultWorld.SystemsManager.CloseWindow();
            }
        }
    }

    public struct SoundComponent : IPleiadComponent
    {
        public string[] files;
    }
    //public class DiceGame// : IPleiadSystem, IRegisterInput
    //{
    //    private bool _started;
    //    readonly SoundPlayer player = new SoundPlayer();
    //    readonly Type soundComponent = typeof(SoundComponent);
    //    readonly EntityManager em = World.DefaultWorld.EntityManager;

    //    int die = 0;

    //    public void StartDialog()
    //    {
    //        _started = true;
    //        var entities = em.GetAllWith(new List<Type>() { soundComponent });
    //        if (entities.Count > 0)
    //        {
    //            //Console.Clear();
    //            InputListener.ClearConsole();
    //            var entity = entities[0];
    //            SoundComponent sc = em.GetComponentData<SoundComponent>(entity);

    //            int select = Roll.Custom(sc.files.Length - 1, lower: 0);
    //            //Console.WriteLine($"DEBUG Selected file {sc.files[select]}");
    //            player.SoundLocation = sc.files[select];


    //            Console.Write("Which die do you want to roll (4, 6, 8, 10, 12, 20)? D");
    //            try
    //            {
    //                die = Convert.ToInt32(Console.ReadLine());

    //                switch (die)
    //                {
    //                    case 4:
    //                        {
    //                            Console.WriteLine($"You rolled {Roll.D4()}");
    //                            player.Play();
    //                            break;
    //                        }
    //                    case 6:
    //                        {
    //                            Console.WriteLine($"You rolled {Roll.D6()}");
    //                            player.Play();
    //                            break;
    //                        }
    //                    case 8:
    //                        {
    //                            Console.WriteLine($"You rolled {Roll.D8()}");
    //                            player.Play();
    //                            break;
    //                        }
    //                    case 10:
    //                        {
    //                            Console.WriteLine($"You rolled {Roll.D10()}");
    //                            player.Play();
    //                            break;
    //                        }
    //                    case 12:
    //                        {
    //                            Console.WriteLine($"You rolled {Roll.D12()}");
    //                            player.Play();
    //                            break;
    //                        }
    //                    case 20:
    //                        {
    //                            Console.WriteLine($"You rolled {Roll.D20()}");
    //                            player.Play();
    //                            break;
    //                        }
    //                    default:
    //                        {
    //                            Console.WriteLine("Wrong input");
    //                            break;
    //                        }
    //                }
    //            }
    //            catch
    //            {
    //                Console.WriteLine("Wrong input");
    //            }


    //            Console.WriteLine("Press R to retry or X to close.");
    //            SystemsManager.WaitForInput(new Key[] { Key.R, Key.X });
    //        }
    //    }

    //    public void Cycle(float dTime)
    //    {
    //        if (!_started)
    //        {
    //            Console.Clear();
    //            Console.WriteLine("/==========================\\");
    //            Console.WriteLine("|                          |");
    //            Console.WriteLine("|                          |");
    //            Console.WriteLine("|     Press R to start     |");
    //            Console.WriteLine("|                          |");
    //            Console.WriteLine("|     Press B to beep      |");
    //            Console.WriteLine("|                          |");
    //            Console.WriteLine("|    Press Esc to exit     |");
    //            Console.WriteLine("|                          |");
    //            Console.WriteLine("|                          |");
    //            Console.WriteLine("\\==========================/");
    //            SystemsManager.WaitForInput(new Key[] { Key.R, Key.Escape, Key.B });
    //        }
    //    }

    //    public void InputRegistration(ref InputListener listener)
    //    {
    //        listener.ListenTo(new Key[] { Key.R, Key.Escape, Key.B });
    //        listener.KeyPress += InputManager;
    //    }
    //    private void InputManager(Key key)
    //    {
    //        switch (key)
    //        {
    //            case Key.R: StartDialog(); break;
    //            case Key.X: _started = false; break;
    //            case Key.B: Console.Beep(); break;
    //        }
    //    }
    //}
}
