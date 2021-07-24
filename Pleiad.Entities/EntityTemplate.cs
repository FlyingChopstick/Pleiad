using System;
using System.Collections.Generic;
using System.Linq;
using Pleiad.Entities.Components;

namespace Pleiad.Entities
{
    public struct EntityTemplate
    {
        public static EntityTemplate Empty { get => new EntityTemplate(new Type[0], new IPleiadComponent[0]); }
        public EntityTemplate(Type[] components, IPleiadComponent[] componentData, bool isSorted = false)
        {
            if (components.Length != componentData.Length)
                throw new ArgumentException("Template arrays must be of the same length and aligned.");

            Components = components;
            ComponentData = componentData;

            if (!isSorted)
            {
                SortComponents();
            }
        }


        public Type[] Components { get; private set; }
        public IPleiadComponent[] ComponentData { get; private set; }

        private void SortComponents()
        {
            var components = Components.ToList();
            List<int> indices = new List<int>();
            List<string> names = new List<string>();
            for (int i = 0; i < Components.Length; i++)
            {
                names.Add(Components[i].Name);
            }
            names.Sort();

            List<Type> compOut = new List<Type>();
            foreach (string name in names)
            {
                Type type = Components.Where(e => e.Name == name).First();
                compOut.Add(type);
                indices.Add(components.IndexOf(type));
            }
            Components = compOut.ToArray();

            IPleiadComponent[] dataOut = new IPleiadComponent[ComponentData.Length];
            for (int i = 0; i < indices.Count; i++)
            {
                dataOut[i] = ComponentData[indices[i]];
            }
        }


        #region Overrides
        public override bool Equals(object obj)
        {
            return obj is EntityTemplate template &&
                   EqualityComparer<Type[]>.Default.Equals(Components, template.Components) &&
                   EqualityComparer<IPleiadComponent[]>.Default.Equals(ComponentData, template.ComponentData);
        }

        public override int GetHashCode()
        {
            int hashCode = 1580228761;
            hashCode = hashCode * -1521134295 + EqualityComparer<Type[]>.Default.GetHashCode(Components);
            hashCode = hashCode * -1521134295 + EqualityComparer<IPleiadComponent[]>.Default.GetHashCode(ComponentData);
            return hashCode;
        }

        public static bool operator ==(EntityTemplate left, EntityTemplate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityTemplate left, EntityTemplate right)
        {
            return !(left == right);
        }
        #endregion
    }
}
