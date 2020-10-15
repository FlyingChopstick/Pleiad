using PleiadExtensions.Arrays;
using PleiadMisc.Comparers;
using System;
using System.Collections.Generic;

namespace PleiadEntities
{
    internal class DataCache
    {
        public DataCache()
        {
            _layout = new Dictionary<Type[], Dictionary<Type, List<int>>>(new TypeArrayComparer());
        }

        private readonly Dictionary<Type[], Dictionary<Type, List<int>>> _layout;

        public void AddIndex(Type[] template, Type component, int index)
        {
            if (!_layout.ContainsKey(template))
            {
                _layout[template] = new Dictionary<Type, List<int>>();
            }
            if (!_layout[template].ContainsKey(component))
            {
                _layout[template][component] = new List<int>();
            }
            if (!_layout[template][component].Contains(index))
            {
                _layout[template][component].Add(index);
            }
        }
        public void RemoveIndex(Type[] template, Type component, int index)
        {
            if (_layout.ContainsKey(template) && _layout[template].ContainsKey(component))
                _layout[template][component].Remove(index);
        }

        public Dictionary<Type, List<int>> GetIndices(Type[] query)
        {
            foreach (var template in _layout.Keys)
            {
                if (query.IsSubsetOf(template))
                {
                    Dictionary<Type, List<int>> result = new Dictionary<Type, List<int>>();

                    foreach (var type in query)
                    {
                        result[type] = _layout[template][type];
                    }

                    return result;
                }
            }


            return default;
        }
        public List<int> GetIndices(Type[] template, Type component)
        {
            if (_layout.ContainsKey(template) && _layout[template].ContainsKey(component))
                return _layout[template][component];

            return default;
        }
    }
}
