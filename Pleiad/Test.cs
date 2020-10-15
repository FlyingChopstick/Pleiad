using PleiadEntities;
using PleiadMisc.Dice;
using PleiadSystems;
using PleiadWorld;
using System;
using System.Collections.Generic;
using System.Media;

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
    public class SoundSystem : IPleiadSystem
    {
        readonly SoundPlayer player = new SoundPlayer();
        readonly Type soundComponent = typeof(SoundComponent);
        readonly EntityManager em = World.DefaultWorld.EntityManager;
        public void Cycle(float dTime)
        {
            Console.Clear();
            var entities = em.GetAllWith(new List<Type>() { soundComponent });
            if (entities.Count > 0)
            {
                var entity = entities[0];
                int select = Roll.DCustom(5, lower: 0);
                SoundComponent sc = em.GetComponentData<SoundComponent>(entity);

                Console.WriteLine($"DEBUG Selected file {sc.files[select]}");
                player.SoundLocation = sc.files[select];

                Console.Write("Which die do you want to roll (4, 6, 8, 10, 12, 20)? D");

                try
                {
                    int die = Convert.ToInt32(Console.ReadLine());

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
                Console.WriteLine("Press enter to retry or e to exit.");
                string input = Console.ReadLine();
                if (input == "e") World.DefaultWorld.StopUpdate();
            }
        }
    }
}
