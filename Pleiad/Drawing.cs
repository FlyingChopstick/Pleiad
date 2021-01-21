using PleiadEntities;
using PleiadInput;
using PleiadSystems;
using PleiadTasks;
using PleiadWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Pleiad
{
    public struct DisplayComponent : IPleiadComponent
    {
        public string output;
        public List<string> queue;
    }
    public struct TextureComponent : IPleiadComponent
    {
        public string texture;
    }
    public struct IsBackgroundComp : IPleiadComponent
    {
        public int id;
    }
    public struct IsCharacterComponent : IPleiadComponent
    {
        public string name;
    }



    public struct DrawingTask : IPleiadTaskOn<DisplayComponent>
    {
        public string frame;

        public void RunOn(int i, ref DisplayComponent[] array)
        {
            array[i].queue.Add(frame);
        }
    }
    public struct DisplayTask : IPleiadTaskOn<DisplayComponent>
    {
        public void RunOn(int i, ref DisplayComponent[] array)
        {
            const int outputWindow = 16;
            char[] outputArr = new char[outputWindow];

            foreach (var item in array[i].queue)
            {
                int s = 0;
                while (s < item.Length && s < outputWindow)
                {
                    outputArr[s] = item[s];
                }
            }

            Console.WriteLine(new string(outputArr));
        }
    }



    //[SysOrder(LoadOrder.LoadFirst)]
    //public class BgrDrawingSystem : IPleiadSystem
    //{
    //    public void Cycle(float dTime)
    //    {
    //        var displayEn = World.DefaultWorld.EntityManager.GetAllWith(new List<Type> { typeof(DisplayComponent) }).FirstOrDefault();
    //        var displayQueue = World.DefaultWorld.EntityManager.GetComponentData<DisplayComponent>(displayEn);

    //        var backgroundEn = World.DefaultWorld.EntityManager.GetAllWith(new List<Type> { typeof(TextureComponent), typeof(IsBackgroundComp) });



    //        var background = World.DefaultWorld.EntityManager.GetComponentData<TextureComponent>(backgroundEn[0]);

    //        displayQueue.queue.Add(background.texture);

    //        World.DefaultWorld.EntityManager.SetComponentData(ref displayEn, typeof(DisplayComponent), displayQueue);
    //    }
    //}

    //[SysOrder(LoadOrder.LoadMiddle)]
    //public class CharacterDrawingTask : IPleiadSystem
    //{
    //    public void Cycle(float dTime)
    //    {
    //        var displayEn = World.DefaultWorld.EntityManager.GetAllWith(new List<Type> { typeof(DisplayComponent) }).FirstOrDefault();
    //        var displayQueue = World.DefaultWorld.EntityManager.GetComponentData<DisplayComponent>(displayEn);

    //        var charEn = World.DefaultWorld.EntityManager.GetAllWith(new List<Type> { typeof(TextureComponent), typeof(IsCharacterComponent) });
    //        var character = World.DefaultWorld.EntityManager.GetComponentData<TextureComponent>(charEn[0]).texture;

    //        displayQueue.queue.Add(character);

    //        World.DefaultWorld.EntityManager.SetComponentData(ref displayEn, typeof(DisplayComponent), displayQueue);
    //    }
    //}




    //public class DisplaySystem : IPleiadSystem
    //{
    //    public void Cycle(float dTime)
    //    {
    //        InputListener.ClearConsole();

    //        var en = World.DefaultWorld.EntityManager.GetAllWith(new List<Type> { typeof(DisplayComponent) }).FirstOrDefault();

    //        var compData = World.DefaultWorld.EntityManager.GetComponentData<DisplayComponent>(en);

    //        const int outputWindow = 16;
    //        char[] outputArr = new char[outputWindow];

    //        foreach (var item in compData.queue)
    //        {
    //            int s = 0;
    //            while (s < item.Length && s < outputWindow)
    //            {
    //                outputArr[s] = item[s];
    //                s++;
    //            }
    //        }

    //        compData.queue = new List<string>();
    //        compData.output = string.Empty;
    //        World.DefaultWorld.EntityManager.SetComponentData(ref en, typeof(DisplayComponent), compData);
    //        Console.WriteLine(new string(outputArr));
    //        Thread.Sleep(200);
    //    }
    //}
}
