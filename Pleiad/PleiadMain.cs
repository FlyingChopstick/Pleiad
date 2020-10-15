using PleiadEntities;
using PleiadMisc.Dice;
using PleiadWorld;
using System;

namespace Pleiad
{
    class PleiadMain
    {
        static void Main(string[] args)
        {
            EntityManager em = World.DefaultWorld.EntityManager;

            EntityTemplate template1 = new EntityTemplate
                (
                new Type[]
                {
                    typeof(TestComponent),
                    typeof(IntTestComponent)
                },
                new IPleiadComponent[]
                {
                    new TestComponent(),
                    new IntTestComponent() { testValue = 13}
                });

            EntityTemplate template2 = new EntityTemplate
                (
                new Type[]
                {
                    typeof(TestComponent)
                },
                new IPleiadComponent[]
                {
                    new TestComponent() { testValue = "hello" }
                });


            EntityTemplate soundTemplate = new EntityTemplate(
                new Type[]
                {
                    typeof(SoundComponent)
                },
                new IPleiadComponent[]
                {
                    new SoundComponent
                    { files = new string[]
                    {
                        @"Sounds\dice1.wav",
                        @"Sounds\dice2.wav",
                        @"Sounds\dice3.wav",
                        @"Sounds\dice4.wav",
                        @"Sounds\dice5.wav",
                        @"Sounds\dice6.wav"
                    }
                    }
                });



            em.AddEntity(soundTemplate);
            em.DEBUG_PrintChunks();


            var dump = em.DEBUG_RetrieveCache(new Type[] { typeof(IntTestComponent) });



            Console.Clear();
            int rolls = 1;
            Console.WriteLine($"D4, {rolls} rolls: {Roll.D4(rolls)}");
            Console.WriteLine($"D6, {rolls} rolls: {Roll.D6(rolls)}");
            rolls = 3;
            Console.WriteLine($"D8, {rolls} rolls: {Roll.D8(rolls)}");
            Console.WriteLine($"D10, {rolls} rolls: {Roll.D10(rolls)}");
            rolls = 1;
            Console.WriteLine($"D12, {rolls} rolls: {Roll.D12(rolls)}");
            Console.WriteLine($"D20, {rolls} rolls: {Roll.D20(rolls)}");
            int faces = 1337;
            rolls = 5;
            Console.WriteLine($"Custom D{faces}, {rolls} rolls: {Roll.DCustom(faces, dice: rolls)}");
            Console.Clear();



            while (World.DefaultWorld.CanUpdate())
            {
            }
        }
    }
}
