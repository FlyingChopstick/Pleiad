using PleiadExtensions.Arrays;
using PleiadMisc.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PleiadEntities
{
    internal class ChunkLookup
    {
        public ChunkLookup()
        {
            _layout = new Dictionary<Type[], Dictionary<Type, List<int>>>(new TypeArrayComparer());
        }

        private readonly Dictionary<Type[], Dictionary<Type, List<int>>> _layout;

        /// <summary>
        /// Adds a new chunk index of some template
        /// </summary>
        /// <param name="template">Chunk Template</param>
        /// <param name="component">Subtype</param>
        /// <param name="index">Chunk index</param>
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
        /// <summary>
        /// Removes a chunk index
        /// </summary>
        /// <param name="template">Chunk template</param>
        /// <param name="component">Subtype</param>
        /// <param name="index">Chunk index</param>
        public void RemoveIndex(Type[] template, Type component, int index)
        {
            if (_layout.ContainsKey(template) && _layout[template].ContainsKey(component))
                _layout[template][component].Remove(index);
        }
        /// <summary>
        /// Get all chunk indices of a template
        /// </summary>
        /// <param name="query">Template query</param>
        /// <returns>A dictionary of chunk indices or default</returns>
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
        public int[] GetIndices(Type query)
        {
            foreach (var template in _layout.Keys)
            {
                if (template.Contains(query))
                {
                    return _layout[template][query].ToArray();
                }
            }

            return default;
        }
        /// <summary>
        /// Get chunk indices for a specified subtype of a template
        /// </summary>
        /// <param name="template">Chunk template</param>
        /// <param name="component">Subtype</param>
        /// <returns>A list of Chunk indices or default</returns>
        public int[] GetIndices(Type[] template, Type component)
        {
            if (_layout.ContainsKey(template) && _layout[template].ContainsKey(component))
                return _layout[template][component].ToArray();

            return default;
        }
    }
}
