using PleiadEntities;
using PleiadInput;
using PleiadMisc.Dice;
using PleiadSystems;
using PleiadWorld;
using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;

namespace Pleiad
{
    public struct TestComponent : IPleiadComponent
    {
        public string testValue;
    }
    public struct IntTestComponent : IPleiadComponent
    {
        public int testValue;
    }
    public struct FloatTestComponent : IPleiadComponent
    {
        public float testValue;
    }




    public struct SoundComponent : IPleiadComponent
    {
        public string[] files;
    }
    public class SoundSystem : IPleiadSystem, IRegisterInput
    {
        private bool _started;
        readonly SoundPlayer player = new SoundPlayer();
        readonly Type soundComponent = typeof(SoundComponent);
        readonly EntityManager em = World.DefaultWorld.EntityManager;

        int die = 0;

        public void StartDialog()
        {
            _started = true;
            var entities = em.GetAllWith(new List<Type>() { soundComponent });
            if (entities.Count > 0)
            {
                //Console.Clear();
                InputListener.ClearConsole();
                var entity = entities[0];
                int select = Roll.DCustom(5, lower: 0);
                SoundComponent sc = em.GetComponentData<SoundComponent>(entity);

                //Console.WriteLine($"DEBUG Selected file {sc.files[select]}");
                player.SoundLocation = sc.files[select];


                Console.Write("Which die do you want to roll (4, 6, 8, 10, 12, 20)? D");
                try
                {
                    die = Convert.ToInt32(Console.ReadLine());

                    switch (die)
                    {
                        case 4:
                            {
                                Console.WriteLine($"You rolled {Roll.D4()}");
                                player.Play();
                                break;
                            }
                        case 6:
                            {
                                Console.WriteLine($"You rolled {Roll.D6()}");
                                player.Play();
                                break;
                            }
                        case 8:
                            {
                                Console.WriteLine($"You rolled {Roll.D8()}");
                                player.Play();
                                break;
                            }
                        case 10:
                            {
                                Console.WriteLine($"You rolled {Roll.D10()}");
                                player.Play();
                                break;
                            }
                        case 12:
                            {
                                Console.WriteLine($"You rolled {Roll.D12()}");
                                player.Play();
                                break;
                            }
                        case 20:
                            {
                                Console.WriteLine($"You rolled {Roll.D20()}");
                                player.Play();
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("Wrong input");
                                break;
                            }
                    }
                }
                catch
                {
                    Console.WriteLine("Wrong input");
                }


                Console.WriteLine("Press R to retry or X to close.");
                SystemsManager.WaitForInput(new Key[] { Key.R, Key.X });
            }
        }

        public void Cycle(float dTime)
        {
            if (!_started)
            {
                Console.Clear();
                Console.WriteLine("/==========================\\");
                Console.WriteLine("|                          |");
                Console.WriteLine("|                          |");
                Console.WriteLine("|     Press R to start     |");
                Console.WriteLine("|                          |");
                Console.WriteLine("|     Press B to beep      |");
                Console.WriteLine("|                          |");
                Console.WriteLine("|    Press Esc to exit     |");
                Console.WriteLine("|                          |");
                Console.WriteLine("|                          |");
                Console.WriteLine("\\==========================/");
                SystemsManager.WaitForInput(new Key[] { Key.R, Key.Escape, Key.B });
            }
        }

        public void Register(ref InputListener listener)
        {
            //sm = World.DefaultWorld.SystemsManager;
            listener.KeyPress += InputManager;
            listener.ListenTo(new Key[] { Key.R, Key.Escape, Key.B });
        }
        private void InputManager(Key key)
        {
            switch (key)
            {
                case Key.R: StartDialog(); break;
                case Key.X: _started = false; break;
                case Key.B: Console.Beep(); break;
            }
        }
    }
}
