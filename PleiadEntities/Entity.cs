using System;
using System.Collections.Generic;
using System.Linq;

namespace PleiadEntities
{
    /// <summary>
    /// A container/reference containing Entity index and ID
    /// </summary>
    public struct Entity
    {
        public Entity(int id, List<Type> components, Dictionary<Type, int> chunks)
        {
            ID = id;
            Components = components;
            Chunks = new Dictionary<Type, int>(chunks);
        }
        public int ID { get; }
        //public Dictionary<Type, int> Chunks { get; }
        public Dictionary<Type, int> Chunks { get; }
        public List<Type> Components { get; private set; }

        public void AddComponent(Type component, int chunkIndex)
        {
            Components.Add(component);
            SortComponents();
            Chunks[component] = chunkIndex;
        }
        public void SortComponents()
        {
            List<int> indices = new List<int>();
            List<string> names = new List<string>();
            for (int i = 0; i < Components.Count; i++)
            {
                names.Add(Components[i].Name);
            }
            names.Sort();

            List<Type> compOut = new List<Type>();
            foreach (string name in names)
            {
                Type type = Components.Where(e => e.Name == name).First();
                compOut.Add(type);
            }
            Components = compOut;
        }
    }
}
