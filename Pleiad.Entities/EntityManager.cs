using System;
using System.Collections.Generic;
using System.Diagnostics;
using Pleiad.Entities.Model;

namespace Pleiad.Entities
{
    /// <summary>
    /// Provides control over Entities and Components
    /// </summary>
    public class EntityManager
    {

        public EntityManager()
        {
            _chunkIndex = new(1);

            _nextID = new(1);
            _chunkSize = DEFAULT_ENTITY_CHUNK_SIZE;
            EntityCount = 0;

            //INIT STORAGE
            _openTypeChunks = new();
            _componentChunks = new();
            _currentChunk = new();

            _entitiesOfType = new();
            _entityTypeChunks = new();

            _lookup = new ChunkLookup();
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
            List<Entity> entities = new();

            for (int entCounter = 0; entCounter < _entitiesOfType[components[0]].Count; entCounter++)
            {
                Entity entity = _entitiesOfType[components[0]][entCounter];
                if (HasAllComponents(entity, components))
                {
                    entities.Add(entity);
                }
            }

            return entities;
        }



        /// <summary>
        /// Removes the Entity
        /// </summary>
        /// <param name="entity">Target</param>
        public void RemoveEntity(Entity entity)
        {
            foreach (var component in _entityTypeChunks[entity].Keys)
            {
                int chunkId = _entityTypeChunks[entity][component];

                _componentChunks[component][chunkId].RemoveEntity(entity);

                _entitiesOfType[component].Remove(entity);
            }

            _entityTypeChunks.Remove(entity);

            EntityCount--;
        }


        /// <summary>
        /// Adds a Component to the Entity
        /// </summary>
        /// <param name="entity">Target</param>
        /// <param name="component">Component type</param>
        /// <param name="componentData">Component data</param>
        public void AddComponent(Entity entity, Type component, IPleiadComponent componentData)
        {
            ChunkIndex chunkIndex = GetEntityComponentChunk(entity, component);

            _componentChunks[component][chunkIndex].AddEntity(entity, componentData);

            MapEntityToType(entity, component, chunkIndex);
        }
        /// <summary>
        /// Removes the Component from the Entity (if Entity has one)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public void RemoveComponent(Entity entity, Type component)
        {
            //ValidateEntity(entity);

            if (!_entitiesOfType.ContainsKey(component)
                || !_entitiesOfType[component].Contains(entity))
            {
                ThrowError(ErrorType.EntityWithoutComponent, entity, component);
            }

            for (int i = 0; i < _componentChunks[component].Count; i++)
            {
                if (TryRemoveEntityFromChunk(entity, component, i))
                {
                    return;
                }
            }


            ThrowError(ErrorType.EntityWithoutComponent, entity, component);
        }

        private bool TryRemoveEntityFromChunk(Entity entity, Type component, int index)
        {
            //RemoveEntity exits with false if the chunk does not contain the entity
            if (_componentChunks[component][index].RemoveEntity(entity))
            {
                RemoveFromLookups(entity, component);
                return true;
            }

            return false;
        }
        private void RemoveFromLookups(Entity entity, Type component)
        {
            _entitiesOfType[component].Remove(entity);
            _entityTypeChunks[entity].Remove(component);
        }


        /// <summary>
        /// Gets the Component data for the specified component or default, if Entity doesn't have this Component
        /// </summary>
        /// <typeparam name="T">Component to retrieve</typeparam>
        /// <param name="entity">Target</param>
        /// <returns>Component data</returns>
        public T GetComponentData<T>(Entity entity) where T : IPleiadComponent
        {
            //ValidateEntity(entity);

            Type component = typeof(T);

            if (!_entitiesOfType.ContainsKey(component))
            {
                ThrowError(ErrorType.NoEntitiesWithComponent, entity, component);
            }

            if (!_entityTypeChunks[entity].ContainsKey(component))
            {
                ThrowError(ErrorType.EntityWithoutComponent, entity, component);
            }

            int chunkId = _entityTypeChunks[entity][component];
            return _componentChunks[component][chunkId].GetComponentData<T>(entity);
        }
        /// <summary>
        /// Sets the data for the existing Component or attaches a new one
        /// </summary>
        /// <param name="entity">Target</param>
        /// <param name="component">Component type</param>
        /// <param name="componentData">Component data</param>
        public void SetComponentData(Entity entity, Type component, IPleiadComponent componentData)
        {
            var chunkId = GetEntityComponentChunk(entity, component);
            _componentChunks[component][chunkId].AddEntity(entity, componentData);
        }


        public DataPack<T> GetTypeData<T>() where T : IPleiadComponent
        {
            //Dictionary<Type, List<int>> query = _lookup.GetIndices(template.Components);
            Type type = typeof(T);
            var chunkIndices = _lookup.GetIndices(type);
            var chunkSizes = new Dictionary<ChunkIndex, int>();
            List<IPleiadComponent> data = new List<IPleiadComponent>();

            foreach (var index in chunkIndices)
            {
                chunkSizes[index] = _componentChunks[type][index].EntityCount;
                data.AddRange(_componentChunks[type][index].GetAllData());
            }

            return new(chunkIndices, chunkSizes, data.ToArray());
        }
        public void SetTypeData<T>(DataPack<T> data) where T : IPleiadComponent
        {
            Type type = typeof(T);
            foreach (var index in data.GetChunkIndices())
            {
                //int chunkSize = data.ChunkSizes[index];

                _componentChunks[type][index].SetAllData(data.GetConvertedData());
            }
        }
        public void SetTypeDataAt<T>(Payload<T> pack) where T : IPleiadComponent
        {
            Type type = typeof(T);
            _componentChunks[type][pack.ChunkIndex].SetDataAt(pack.Data);
        }



        private bool HasAllComponents(Entity entity, List<Type> components)
        {
            foreach (var component in components)
            {
                if (!_entitiesOfType.ContainsKey(component))
                {
                    return false;
                }

                if (!_entitiesOfType[component].Contains(entity))
                {
                    return false;
                }
            }

            return true;
        }
        private Entity CreateEntity(EntityTemplate template)
        {
            var chunks = new Dictionary<Type, int>();
            for (int i = 0; i < template.Components.Length; i++)
            {
                var component = template.Components[i];
                ChunkIndex chunkIndex = GetComponentChunk(component);
                Entity newEntity = new(_nextID);
                _componentChunks[component][chunkIndex].AddEntity(newEntity, template.ComponentData[i]);
                chunks[component] = chunkIndex;

                _lookup.AddIndex(template.Components, component, chunkIndex);

                MapEntityToType(newEntity, component, chunkIndex);
            }

            EntityCount++;
            return new Entity(_nextID++);
        }
        private void MapEntityToType(Entity entity, Type component, ChunkIndex chunkIndex)
        {
            if (!_entitiesOfType.ContainsKey(component))
            {
                _entitiesOfType.Add(component, new());
            }
            if (!_entitiesOfType[component].Contains(entity))
            {
                _entitiesOfType[component].Add(entity);
            }


            if (!_entityTypeChunks.ContainsKey(entity))
            {
                _entityTypeChunks.Add(entity, new());
            }

            _entityTypeChunks[entity].Add(component, chunkIndex);
        }
        private Dictionary<Type, ChunkIndex> GetEntityChunks(Entity entity, List<Type> components)
        {
            var chunks = new Dictionary<Type, ChunkIndex>();
            for (int compCounter = 0; compCounter < components.Count; compCounter++)
            {
                chunks.Add(components[compCounter], FindComponentChunk(entity, components[compCounter]));
            }

            return chunks;
        }




        private ChunkIndex GetComponentChunk(Type chunkType)
        {
            //if there is a chunk of that type selected
            if (_currentChunk.ContainsKey(chunkType))
            {
                ChunkIndex index = _currentChunk[chunkType];
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
        private ChunkIndex CreateComponentChunk(Type component, int chunkSize = -1)
        {
            if (chunkSize <= 0) chunkSize = DEFAULT_ENTITY_CHUNK_SIZE;
            EntityChunk newChunk = new EntityChunk(_chunkIndex++, chunkSize, component);

            if (!_componentChunks.ContainsKey(component))
            {
                _componentChunks[component] = new();
                _openTypeChunks[component] = new();
            }
            _componentChunks[component].Add(newChunk);

            return new(_componentChunks[component].IndexOf(newChunk));
        }
        private ChunkIndex GetEntityComponentChunk(Entity entity, Type component)
        {
            if (!_componentChunks.ContainsKey(component))
            {
                return GetComponentChunk(component);
            }

            for (int i = 0; i < _componentChunks[component].Count; i++)
            {
                if (_componentChunks[component][i].ContainsEntity(entity))
                {
                    return new(i);
                }
            }

            return GetComponentChunk(component);
        }

        private ChunkIndex FindComponentChunk(Entity entity, Type chunkType)
        {
            for (int i = 0; i < _componentChunks[chunkType].Count; i++)
            {
                var chunk = _componentChunks[chunkType][i];
                if (chunk.ContainsEntity(entity))
                {
                    return new(i);
                }
            }

            ThrowError(ErrorType.EntityWithoutComponent, entity, chunkType);
            //this will not be returned, unless ThrowError is supressed here
            return new();
        }

        private enum ErrorType
        {
            IndexOutOfRange = 0,
            CouldNotAddEntity = 1,
            CouldNotRemoveEntity = 2,
            CouldNotSetComp = 3,
            CouldNotRemoveComp = 4,
            CompDoesNotExist = 5,
            EntityWithoutComponent = 6,
            NoEntitiesWithComponent = 7,
        }
        private void ThrowError(ErrorType errorType, Entity entity, Type component = null, bool supress = false)
        {
            var st = new StackTrace();
            var caller = st.GetFrame(1).GetMethod().Name;
            st = new StackTrace(1, true);
            var line = $"File: {st.GetFrame(1).GetFileName()}, line {st.GetFrame(1).GetFileLineNumber()}";

            switch (errorType)
            {
                case ErrorType.IndexOutOfRange:
                    {
                        LogAndThrow(st, $"Could not resolve the chunk index for Entity ID{entity.ID} in {caller}, {line}", supress);
                        return;
                    }
                case ErrorType.CouldNotAddEntity:
                    {
                        LogAndThrow(st, $"Could not add Entity ID{entity.ID} in {caller}, {line}", supress);
                        return;
                    }
                case ErrorType.CouldNotRemoveEntity:
                    {
                        LogAndThrow(st, $"Could not remove Entity ID{entity.ID} in {caller}, {line}", supress);
                        return;
                    }
                case ErrorType.CouldNotSetComp:
                    {
                        LogAndThrow(st, $"Could not set the component for Entity ID{entity.ID} in {caller}, {line}", supress);
                        return;
                    }
                case ErrorType.CouldNotRemoveComp:
                    {
                        LogAndThrow(st, $"Could not remove the component for Entity ID{entity.ID} in {caller}, {line}", supress);
                        return;
                    }
                case ErrorType.CompDoesNotExist:
                    {
                        LogAndThrow(st, $"There are no chunks of type {component} in {caller}, {line}", supress);
                        return;
                    }
                case ErrorType.EntityWithoutComponent:
                    {
                        LogAndThrow(st, $"Entity {entity.ID} does not have the {component.Name} component", supress);
                        return;
                    }
                case ErrorType.NoEntitiesWithComponent:
                    {
                        LogAndThrow(st, $"There are no entities with {component.Name} component", supress);
                        return;
                    }
            }
        }
        private static void LogAndThrow(StackTrace st, string message, bool supress = false)
        {
            Console.WriteLine(message);
            Console.WriteLine($"Stack trace:\n{st}");
            if (!supress)
            {
                throw new ArgumentException(message);
            }
        }


        private ChunkIndex _chunkIndex;
        private int _chunkSize;
        private Entity _nextID;

        private readonly Dictionary<Type, Stack<ChunkIndex>> _openTypeChunks;
        private readonly Dictionary<Type, List<EntityChunk>> _componentChunks;
        private readonly Dictionary<Type, ChunkIndex> _currentChunk;

        private readonly Dictionary<Type, List<Entity>> _entitiesOfType;
        private readonly Dictionary<Entity, Dictionary<Type, ChunkIndex>> _entityTypeChunks;

        private readonly ChunkLookup _lookup;

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

        public Dictionary<Type, List<ChunkIndex>> DEBUG_RetrieveCache(Type[] query)
        {
            return _lookup.GetIndices(query);
        }
#endif
        #endregion
    }
}
