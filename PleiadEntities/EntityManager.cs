using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PleiadEntities
{
    /// <summary>
    /// Provides control over Entities and Components
    /// </summary>
    public class EntityManager
    {

        public EntityManager()
        {
            _chunkIndex = 1;
            _nextID = 1;
            _chunkSize = DEFAULT_ENTITY_CHUNK_SIZE;
            EntityCount = 0;

            //INIT STORAGE
            _openTypeChunks = new Dictionary<Type, Stack<int>>();
            _componentChunks = new Dictionary<Type, List<EntityChunk>>();
            _currentChunk = new Dictionary<Type, int>();

            _cache = new DataCache();
        }
        /// <summary>
        /// Adds an empty Entity
        /// </summary>
        /// <returns>Entity handle</returns>
        public Entity AddEntity()
        {
            return CreateEntity(EntityTemplate.Empty);
        }
        /// <summary>
        /// Adds an Entity using a Template
        /// </summary>
        /// <param name="template">Entity template</param>
        /// <returns>Entity handle</returns>
        public Entity AddEntity(EntityTemplate template)
        {
            return CreateEntity(template);
        }
        /// <summary>
        /// Adds an Entity using arrays of Component types and data
        /// </summary>
        /// <param name="components">Array of Component types</param>
        /// <param name="componentData">Array of Component data</param>
        /// <returns>Entity handle</returns>
        public Entity AddEntity(Type[] components, IPleiadComponent[] componentData)
        {
            return CreateEntity(new EntityTemplate(components, componentData));
        }
        /// <summary>
        /// Adds an Entity with Components and no data
        /// </summary>
        /// <param name="components">Array of Component types</param>
        /// <returns>Entity handle</returns>
        public Entity AddEntity(Type[] components)
        {
            return CreateEntity(new EntityTemplate(components, new IPleiadComponent[components.Length]));
        }

        public List<Entity> GetAllWith(List<Type> components)
        {
            List<Entity> entities = new List<Entity>();
            //Dictionary<int, List<Type>> search = new Dictionary<int, List<Type>>();
            //Dictionary<Type, List<int>> chunks = new Dictionary<Type, List<int>>();
            List<int> IDs = new List<int>();

            var firstComp = components[0];
            if (_componentChunks.ContainsKey(firstComp))
            {
                IDs = new List<int>();
                for (int j = 0; j < _componentChunks[firstComp].Count; j++)
                {
                    IDs.AddRange(_componentChunks[firstComp][j].ChunkIDs);
                }
            }


            List<int> resultID = new List<int>();
            foreach (var id in IDs)
            {
                if (ID_HasAllComponents(id, components))
                {
                    if (!resultID.Contains(id))
                    {
                        resultID.Add(id);
                        Dictionary<Type, int> outChunks = new Dictionary<Type, int>();
                        foreach (var component in components)
                        {
                            outChunks[component] = ID_ComponentChunk(id, component);
                        }
                        entities.Add(new Entity(id, components, outChunks));
                    }
                }
            }

            return entities;
        }


        private bool ID_HasAllComponents(int id, List<Type> components)
        {
            foreach (var component in components)
            {
                if (!ID_HasComponent(id, component)) return false;
            }
            return true;
        }
        private bool ID_HasComponent(int id, Type component)
        {
            if (_componentChunks.ContainsKey(component))
            {
                foreach (var chunk in _componentChunks[component])
                {
                    if (chunk.IsInChunk(id)) return true;
                }
            }
            return false;
        }
        private int ID_ComponentChunk(int id, Type component)
        {
            if (_componentChunks.ContainsKey(component))
            {
                for (int i = 0; i < _componentChunks[component].Count; i++)
                {
                    if (_componentChunks[component][i].IsInChunk(id))
                        return i;
                }
            }

            return -1;
        }
        private Entity CreateEntity(EntityTemplate template)
        {
            var chunks = new Dictionary<Type, int>();
            for (int i = 0; i < template.Components.Length; i++)
            {
                var component = template.Components[i];
                int index = GetComponentChunk(component);
                _componentChunks[component][index].AddEntity(_nextID, template.ComponentData[i]);
                chunks[component] = index;

                _cache.AddIndex(template.Components, component, index);
            }

            EntityCount++;
            return new Entity(_nextID++, template.Components.ToList(), chunks);
        }
        /// <summary>
        /// Removes the Entity
        /// </summary>
        /// <param name="entity">Target</param>
        public void RemoveEntity(Entity entity)
        {
            ValidateEntity(entity);

            foreach (Type component in entity.Chunks.Keys)
            {
                int chunkIndex = entity.Chunks[component];
                _componentChunks[component][chunkIndex].RemoveEntity(entity.ID);
                _openTypeChunks[component].Push(chunkIndex);
            }
            EntityCount--;
        }
        /// <summary>
        /// Adds a Component to the Entity
        /// </summary>
        /// <param name="entity">Target</param>
        /// <param name="component">Component type</param>
        /// <param name="componentData">Component data</param>
        public void AddComponent(ref Entity entity, Type component, IPleiadComponent componentData)
        {
            ValidateEntity(entity);

            int index;
            if (entity.Components.Contains(component))
            {
                index = entity.Chunks[component];
            }
            else
            {
                index = GetComponentChunk(component);
                entity.AddComponent(component, index);
            }

            _componentChunks[component][index].SetComponentData(entity.ID, componentData);
        }
        /// <summary>
        /// Removes the Component from the Entity (if Entity has one)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public void RemoveComponent(ref Entity entity, Type component)
        {
            ValidateEntity(entity);

            if (entity.Components.Contains(component))
            {
                if (_componentChunks.ContainsKey(component))
                {
                    _componentChunks[component][entity.Chunks[component]].RemoveEntity(entity.ID);
                    entity.Components.Remove(component);
                }
                else
                    ThrowError(ErrorType.CompDoesNotExist, entity, component);
            }
        }
        /// <summary>
        /// Gets the Component data for the specified component or default, if Entity doesn't have this Component
        /// </summary>
        /// <typeparam name="T">Component to retrieve</typeparam>
        /// <param name="entity">Target</param>
        /// <returns>Component data</returns>
        public T GetComponentData<T>(Entity entity) where T : IPleiadComponent
        {
            ValidateEntity(entity);

            Type component = typeof(T);
            if (_componentChunks.ContainsKey(component))
            {
                return _componentChunks[component][entity.Chunks[component]].GetComponentData<T>(entity);
            }
            else
            {
                ThrowError(ErrorType.CompDoesNotExist, entity, component);
                return default;
            }
        }
        /// <summary>
        /// Sets the data for the existing Component or attaches a new one
        /// </summary>
        /// <param name="entity">Target</param>
        /// <param name="component">Component type</param>
        /// <param name="componentData">Component data</param>
        public void SetComponentData(ref Entity entity, Type component, IPleiadComponent componentData)
        {
            if (entity.Components.Contains(component))
            {
                ValidateEntity(entity);

                _componentChunks[component][entity.Chunks[component]].SetComponentData(entity.ID, componentData);
            }
            else
                AddComponent(ref entity, component, componentData);
        }

        /// <summary>
        /// Checks that _typeChunks contains all components of the entity and that the index in entity.Chunks is appropriate
        /// </summary>
        /// <param name="entity">Target</param>
        private void ValidateEntity(Entity entity)
        {
            foreach (Type component in entity.Chunks.Keys)
            {
                //if _typeChunks does not have that component, 
                //or chunk index in entity is higher than the count of _typeChunks of that component,
                //Throw error
                if (!_componentChunks.ContainsKey(component))
                    ThrowError(ErrorType.CompDoesNotExist, entity, component);

                if (_componentChunks[component].Count <= entity.Chunks[component])
                    ThrowError(ErrorType.IndexOutOfRange, entity);
            }
        }
        private int GetComponentChunk(Type chunkType)
        {
            //if there is a chunk of that type selected
            if (_currentChunk.ContainsKey(chunkType))
            {
                int index = _currentChunk[chunkType];
                //if that chunk is not full
                if (!_componentChunks[chunkType][index].IsFull)
                    return index;
            }
            else
            {
                //if a stack of open chunks has that type AND the stack is >0
                if (_openTypeChunks.ContainsKey(chunkType) && _openTypeChunks[chunkType].Count > 0)
                {
                    _currentChunk[chunkType] = _openTypeChunks[chunkType].Pop();
                    return _currentChunk[chunkType];
                }
            }

            //otherwise create a new chunk
            _currentChunk[chunkType] = CreateComponentChunk(chunkType);
            return _currentChunk[chunkType];
        }
        private int CreateComponentChunk(Type component, int chunkSize = -1)
        {
            if (chunkSize <= 0) chunkSize = DEFAULT_ENTITY_CHUNK_SIZE;
            EntityChunk newChunk = new EntityChunk(_chunkIndex++, chunkSize, component);

            if (!_componentChunks.ContainsKey(component))
            {
                _componentChunks[component] = new List<EntityChunk>();
                _openTypeChunks[component] = new Stack<int>();
            }
            _componentChunks[component].Add(newChunk);

            return _componentChunks[component].IndexOf(newChunk);
        }


        private enum ErrorType
        {
            IndexOutOfRange = 0,
            CouldNotAddEntity = 1,
            CouldNotRemoveEntity = 2,
            CouldNotSetComp = 3,
            CouldNotRemoveComp = 4,
            CompDoesNotExist = 5,
        }
        private void ThrowError(ErrorType errorType, Entity entity, Type component = null)
        {
            var st = new StackTrace(true);
            var caller = st.GetFrame(1).GetMethod().Name;
            var line = $"File: {st.GetFrame(1).GetFileName()}, line {st.GetFrame(1).GetFileLineNumber()}";

            switch (errorType)
            {
                case ErrorType.IndexOutOfRange:
                    {
                        Console.WriteLine($"Could not resolve the chunk index for Entity ID{entity.ID} in {caller}, {line}");
                        Console.WriteLine($"Stack trace: {st}");
                        throw new ArgumentException($"Could not resolve the chunk index for Entity ID{entity.ID} in {caller}, {line}");
                    }
                case ErrorType.CouldNotAddEntity:
                    {
                        Console.WriteLine($"Could not add Entity ID{entity.ID} in {caller}, {line}");
                        Console.WriteLine($"Stack trace: {st}");
                        throw new ArgumentException($"Could not add Entity ID{entity.ID} in {caller}, {line}");
                    }
                case ErrorType.CouldNotRemoveEntity:
                    {
                        Console.WriteLine($"Could not remove Entity ID{entity.ID} in {caller}, {line}");
                        Console.WriteLine($"Stack trace: {st}");
                        throw new ArgumentException($"Could not remove Entity ID{entity.ID} in {caller}, {line}");
                    }
                case ErrorType.CouldNotSetComp:
                    {
                        Console.WriteLine($"Could not set the component for Entity ID{entity.ID} in {caller}, {line}");
                        Console.WriteLine($"Stack trace: {st}");
                        throw new ArgumentException($"Could not set the component for Entity ID{entity.ID} in {caller}, {line}");
                    }
                case ErrorType.CouldNotRemoveComp:
                    {
                        Console.WriteLine($"Could not remove the component for Entity ID{entity.ID} in {caller}, {line}");
                        Console.WriteLine($"Stack trace: {st}");
                        throw new ArgumentException($"Could not remove the component for Entity ID{entity.ID} in {caller}, {line}");
                    }
                case ErrorType.CompDoesNotExist:
                    {
                        Console.WriteLine($"There are no chunks of type {component} in {caller}, {line}");
                        Console.WriteLine($"Stack trace: {st}");
                        throw new ArgumentException($"There are no chunks of type {component} in {caller}, {line}");
                    }

            }
        }

        /// <summary>
        /// Default size of an Entity Chunk
        /// </summary>
        public const int DEFAULT_ENTITY_CHUNK_SIZE = 20;
        /// <summary>
        /// Gets or sets the new Chunk capacity
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public int ChunkSize
        {
            get => _chunkSize;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException($"Chunk size must be >0 (tried {value})");
                _chunkSize = value;
            }
        }
        public int EntityCount { get; private set; }

        private int _chunkIndex;
        private int _chunkSize;
        private int _nextID;

        private readonly Dictionary<Type, Stack<int>> _openTypeChunks;
        private readonly Dictionary<Type, List<EntityChunk>> _componentChunks;
        private readonly Dictionary<Type, int> _currentChunk;

        private readonly DataCache _cache;

        #region DebugFunctions
#if DEBUG
        /// <summary>
        /// Prints existing chunks to the console
        /// </summary>
        public void DEBUG_PrintChunks(bool wait = false)
        {
            if (wait) { Console.WriteLine("Press Enter to continue."); Console.ReadLine(); }
            Console.Clear();
            foreach (var type in _componentChunks.Keys)
            {
                foreach (var chunk in _componentChunks[type])
                {
                    chunk.DEBUG_PrintEntities();
                }
            }
        }

        public Dictionary<Type, List<int>> DEBUG_RetrieveCache(Type[] query)
        {
            return _cache.GetIndices(query);
        }
#endif
        #endregion
    }
}
