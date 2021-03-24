using PleiadMisc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PleiadEntities
{
    /// <summary>
    /// Provides control over Entities and Components
    /// </summary>
    public class EntityManager
    {
        /// <summary>
        /// All chunks
        /// </summary>
        private readonly List<EntityChunk> _chunks;
        /// <summary>
        /// Type - list of chunk indices lookup
        /// </summary>
        private readonly IIndexLookup<Type> _chunkLUP;
        /// <summary>
        /// Type - list of open chunk indices lookup
        /// </summary>
        private readonly IIndexLookup<Type> _openTypeChunks;

        /// <summary>
        /// Entity ID - list of chunks containing this entity lookup
        /// </summary>
        private readonly IIndexLookup<int> _entityChunks;
        /// <summary>
        /// Entity ID - [Dictionary of Component - chunk index]
        /// </summary>
        private readonly Dictionary<int, Dictionary<Type, int>> _entityComponentStorage;
        /// <summary>
        /// Entity ID - List of entity components
        /// </summary>
        private readonly Dictionary<int, HashSet<Type>> _entityComponents;

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
        /// <summary>
        /// Total amount of entities 
        /// </summary>
        public int EntityCount { get; private set; }
        /// <summary>
        /// Total amount of chunks
        /// </summary>
        public int ChunkCount { get; private set; }

        /// <summary>
        /// Index for new chunk
        /// </summary>
        private int _nextChunkIndex;
        /// <summary>
        /// Capacity of the new chunk
        /// </summary>
        private int _chunkSize;
        /// <summary>
        /// ID of next new entity
        /// </summary>
        private int _nextID;



        public EntityManager()
        {
            _nextChunkIndex = 0;
            _nextID = 1;
            _chunkSize = DEFAULT_ENTITY_CHUNK_SIZE;

            EntityCount = 0;
            ChunkCount = 0;

            //INIT STORAGE
            //_openTypeChunks = new Dictionary<Type, Stack<int>>();
            //_componentChunks = new Dictionary<Type, List<IEntityChunk>>();
            //_currentChunk = new Dictionary<Type, int>();

            _chunks = new List<EntityChunk>();
            _chunkLUP = new ChunkLookup();
            _openTypeChunks = new ChunkLookup();
            _entityChunks = new EntityChunksLookup();
            _entityComponentStorage = new Dictionary<int, Dictionary<Type, int>>();
            _entityComponents = new Dictionary<int, HashSet<Type>>();
        }
        /// <summary>
        /// Adds an empty Entity
        /// </summary>
        /// <returns>Entity handle</returns>
        //public Entity AddEntity()
        //{
        //    return CreateEntity(EntityTemplate.Empty);
        //}
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

        //public List<Entity> GetAllWith(List<Type> components)
        //{
        //    List<Entity> entities = new List<Entity>();
        //    //Dictionary<int, List<Type>> search = new Dictionary<int, List<Type>>();
        //    //Dictionary<Type, List<int>> chunks = new Dictionary<Type, List<int>>();
        //    List<int> IDs = new List<int>();

        //    var firstComp = components[0];
        //    if (_componentChunks.ContainsKey(firstComp))
        //    {
        //        IDs = new List<int>();
        //        for (int j = 0; j < _componentChunks[firstComp].Count; j++)
        //        {
        //            IDs.AddRange(_componentChunks[firstComp][j].ChunkIDs);
        //        }
        //    }


        //    List<int> resultID = new List<int>();
        //    foreach (var id in IDs)
        //    {
        //        if (ID_HasAllComponents(id, components))
        //        {
        //            if (!resultID.Contains(id))
        //            {
        //                resultID.Add(id);
        //                Dictionary<Type, int> outChunks = new Dictionary<Type, int>();
        //                foreach (var component in components)
        //                {
        //                    outChunks[component] = ID_ComponentChunk(id, component);
        //                }
        //                entities.Add(new Entity(id, components, outChunks));
        //            }
        //        }
        //    }

        //    return entities;
        //}


        //private bool ID_HasAllComponents(int id, List<Type> components)
        //{
        //    foreach (var component in components)
        //    {
        //        if (!ID_HasComponent(id, component)) return false;
        //    }
        //    return true;
        //}
        //private bool ID_HasComponent(int id, Type component)
        //{
        //    if (_componentChunks.ContainsKey(component))
        //    {
        //        foreach (var chunk in _componentChunks[component])
        //        {
        //            if (chunk.ContainsID(id)) return true;
        //        }
        //    }
        //    return false;
        //}
        //private int ID_ComponentChunk(int id, Type component)
        //{
        //    if (_componentChunks.ContainsKey(component))
        //    {
        //        for (int i = 0; i < _componentChunks[component].Count; i++)
        //        {
        //            if (_componentChunks[component][i].ContainsID(id))
        //                return i;
        //        }
        //    }

        //    return -1;
        //}
        private Entity CreateEntity(EntityTemplate template)
        {
            //init storage
            int enID = _nextID++;
            EntityCount++;
            _entityComponentStorage[enID] = new Dictionary<Type, int>();
            _entityComponents[enID] = new HashSet<Type>();
            //add each component
            for (int i = 0; i < template.Components.Length; i++)
            {
                var data = template.ComponentData[i];
                var component = template.Components.Where(c => c == data.GetType()).First();


                int chunkIndex = GetComponentChunk(component);
                _chunks[chunkIndex].AddEntity(enID);
                if (_chunks[chunkIndex].IsFull)
                    _openTypeChunks.RemoveIndex(component, chunkIndex);

                AddComponent(enID, component, data);
            }

            //return entity handle
            return new Entity(enID);
        }
        /// <summary>
        /// Removes the Entity
        /// </summary>
        /// <param name="entity">Target</param>
        public void RemoveEntity(Entity entity)
        {
            var entityChunks = _entityChunks.GetIndices(entity.ID);
            if (entityChunks.IsFound)
            {
                foreach (var chunkIndex in entityChunks.Data)
                {
                    _chunks[chunkIndex].RemoveEntity(entity.ID);
                }
                _entityChunks.RemoveKey(entity.ID);
                _entityComponentStorage.Remove(entity.ID);
                _entityComponents.Remove(entity.ID);
                EntityCount--;
            }
        }
        /// <summary>
        /// Adds a Component to the Entity
        /// </summary>
        /// <param name="entity">Target</param>
        /// <param name="component">Component type</param>
        /// <param name="componentData">Component data</param>
        public void AddComponent(Entity entity, Type component, IPleiadComponent componentData)
        {
            AddComponent(entity.ID, component, componentData);
        }
        private void AddComponent(int entityID, Type component, IPleiadComponent componentData)
        {
            int chunkIndex;
            var componentChunks = _chunkLUP.GetIndices(component);

            //if there's no free component chunk
            if (!componentChunks.IsFound || componentChunks.Data.Count == 0)
            {
                //create new chunk
                chunkIndex = GetComponentChunk(component);
            }
            else
                chunkIndex = componentChunks.Data.First();

            //set data
            _chunks[chunkIndex].AddEntity(entityID);
            _chunks[chunkIndex].SetComponentData(entityID, componentData);
            if (_chunks[chunkIndex].IsFull)
                _openTypeChunks.RemoveIndex(component, chunkIndex);

            //update entity components
            _entityComponents[entityID].Add(component);
            //update entity chunks
            _entityChunks.AddIndex(entityID, chunkIndex);
            //update entity component storage
            _entityComponentStorage[entityID][component] = chunkIndex;
        }
        /// <summary>
        /// Removes the Component from the Entity (if Entity has one)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        public void RemoveComponent(Entity entity, Type component)
        {
            //ValidateEntity(entity);

            //if (entity.Components.Contains(component))
            if (_entityComponents.ContainsKey(entity.ID))
            {
                if (_entityComponents[entity.ID].Contains(component))
                {
                    var chunkIndex = _entityComponentStorage[entity.ID][component];
                    _chunks[chunkIndex].RemoveEntity(entity.ID);
                    _entityChunks.RemoveIndex(entity.ID, chunkIndex);
                    _entityComponents[entity.ID].Remove(component);

                    //update entity components
                    _entityComponents[entity.ID].Remove(component);
                    //update entity chunks
                    _entityChunks.RemoveIndex(entity.ID, chunkIndex);
                    //update entity component storage
                    _entityComponentStorage[entity.ID].Remove(component);
                }
            }
        }
        /// <summary>
        /// Gets the Component data for the specified component or default, if Entity doesn't have this Component
        /// </summary>
        /// <typeparam name="T">Component to retrieve</typeparam>
        /// <param name="entity">Target</param>
        /// <returns>Component data</returns>
        public Result<T> GetComponentData<T>(Entity entity)
        {
            Type dataType = typeof(T);

            if (_entityComponentStorage.ContainsKey(entity.ID))
            {
                if (_entityComponentStorage[entity.ID].ContainsKey(dataType))
                {
                    int index = _entityComponentStorage[entity.ID][dataType];
                    if (_chunks[index].ChunkType == dataType)
                    {
                        var ipc = _chunks[index].GetComponentData(entity.ID);

                        if (ipc.IsFound)
                            return Result<T>.Found((T)Convert.ChangeType((T)ipc.Data, dataType));
                    }
                }
            }

            //ThrowError(ErrorType.CompDoesNotExist, entity, dataType);
            //return default;
            return Result<T>.NotFound;
        }
        /// <summary>
        /// Sets the data for the existing Component or attaches a new one
        /// </summary>
        /// <param name="entity">Target</param>
        /// <param name="component">Component type</param>
        /// <param name="componentData">Component data</param>
        public void SetComponentData(Entity entity, Type component, IPleiadComponent componentData)
        {
            //check if entity has component
            //  if not, add component
            //  if has, update data

            if (_entityComponentStorage.ContainsKey(entity.ID))
            {
                int chunkIndex;

                //if entity has the component
                if (_entityComponents[entity.ID].Contains(component))
                {
                    //get it's storage index
                    chunkIndex = _entityComponentStorage[entity.ID][component];
                    //set data
                    _chunks[chunkIndex].SetComponentData(entity.ID, componentData);
                }
                //if not
                else
                {
                    //add component
                    AddComponent(entity.ID, component, componentData);
                }
            }
        }


        /// <summary>
        /// Get a list of all chunk indices of selected component
        /// </summary>
        /// <param name="chunkType">Component</param>
        /// <returns>HashSet of chunk indices</returns>
        public Result<HashSet<int>> GetAllChunksOfType(Type chunkType)
        {
            return _chunkLUP.GetIndices(chunkType);
        }
        /// <summary>
        /// Get all data from the chunk
        /// </summary>
        /// <param name="chunkIndex">Selected chunk</param>
        /// <returns>List of IPleiadComponents</returns>
        public Result<List<IPleiadComponent>> GetChunkData(int chunkIndex)
        {
            if (chunkIndex <= _chunks.Count && chunkIndex >= 0)
            {
                return Result<List<IPleiadComponent>>.Found(_chunks[chunkIndex].GetChunkData());
            }
            else
                return Result<List<IPleiadComponent>>.NotFound;
        }
        /// <summary>
        /// Set the data in the chunk
        /// </summary>
        /// <typeparam name="T">Component</typeparam>
        /// <param name="chunkIndex">Selected chunk</param>
        /// <param name="data">List of data</param>
        public void SetChunkData<T>(int chunkIndex, List<T> data) where T : IPleiadComponent
        {
            if (chunkIndex <= _chunks.Count && chunkIndex >= 0
                && _chunks[chunkIndex].ChunkType == typeof(T))
            {
                _chunks[chunkIndex].SetChunkData(data);
            }
        }
        /// <summary>
        /// Set the data in the chunk
        /// </summary>
        /// <typeparam name="T">Component</typeparam>
        /// <param name="chunkIndex">Selected chunk</param>
        /// <param name="data">List of data</param>
        public void SetChunkData<T>(int chunkIndex, T[] data) where T : IPleiadComponent
        {
            if (chunkIndex <= _chunks.Count && chunkIndex >= 0
                && _chunks[chunkIndex].ChunkType == typeof(T))
            {
                _chunks[chunkIndex].SetChunkData(data);
            }
        }



        /// <summary>
        /// Get the index of the existing open component chunk or create a new one
        /// </summary>
        /// <param name="chunkType">Compoenent</param>
        /// <returns>Index of an open chunk</returns>
        private int GetComponentChunk(Type chunkType)
        {
            int chunkIndex;

            var openChunk = _openTypeChunks.GetIndices(chunkType);
            if (openChunk.IsFound && openChunk.Data.Count > 0)
                chunkIndex = openChunk.Data.First();
            else
            {
                chunkIndex = CreateComponentChunk(chunkType);

                //var res = _chunkLUP.GetIndices(chunkType);
                //if (!res.IsFound || res.Data.Count == 0)
                //    chunkIndex = CreateComponentChunk(chunkType);
                //else
                //    chunkIndex = res.Data.First();
            }
            return chunkIndex;
        }
        /// <summary>
        /// Create new component chunk, register it as open
        /// </summary>
        /// <param name="component">Component</param>
        /// <param name="chunkSize">Size of the new chunk</param>
        /// <returns>Index of the new chunk</returns>
        private int CreateComponentChunk(Type component, int chunkSize = -1)
        {
            if (chunkSize <= 0) chunkSize = DEFAULT_ENTITY_CHUNK_SIZE;
            EntityChunk newChunk = new EntityChunk(_nextChunkIndex, component, chunkSize);
            _chunks.Add(newChunk);
            _chunkLUP.AddIndex(component, _nextChunkIndex);
            _openTypeChunks.AddIndex(component, _nextChunkIndex);

            ChunkCount++;
            return _nextChunkIndex++;
        }

        //private enum ErrorType
        //{
        //    IndexOutOfRange = 0,
        //    CouldNotAddEntity = 1,
        //    CouldNotRemoveEntity = 2,
        //    CouldNotSetComp = 3,
        //    CouldNotRemoveComp = 4,
        //    CompDoesNotExist = 5,
        //}
        //private void ThrowError(ErrorType errorType, Entity entity, Type component = null)
        //{
        //    var st = new StackTrace(true);
        //    var caller = st.GetFrame(1).GetMethod().Name;
        //    var line = $"File: {st.GetFrame(1).GetFileName()}, line {st.GetFrame(1).GetFileLineNumber()}";

        //    switch (errorType)
        //    {
        //        case ErrorType.IndexOutOfRange:
        //            {
        //                Console.WriteLine($"Could not resolve the chunk index for Entity ID{entity.ID} in {caller}, {line}");
        //                Console.WriteLine($"Stack trace: {st}");
        //                throw new ArgumentException($"Could not resolve the chunk index for Entity ID{entity.ID} in {caller}, {line}");
        //            }
        //        case ErrorType.CouldNotAddEntity:
        //            {
        //                Console.WriteLine($"Could not add Entity ID{entity.ID} in {caller}, {line}");
        //                Console.WriteLine($"Stack trace: {st}");
        //                throw new ArgumentException($"Could not add Entity ID{entity.ID} in {caller}, {line}");
        //            }
        //        case ErrorType.CouldNotRemoveEntity:
        //            {
        //                Console.WriteLine($"Could not remove Entity ID{entity.ID} in {caller}, {line}");
        //                Console.WriteLine($"Stack trace: {st}");
        //                throw new ArgumentException($"Could not remove Entity ID{entity.ID} in {caller}, {line}");
        //            }
        //        case ErrorType.CouldNotSetComp:
        //            {
        //                Console.WriteLine($"Could not set the component for Entity ID{entity.ID} in {caller}, {line}");
        //                Console.WriteLine($"Stack trace: {st}");
        //                throw new ArgumentException($"Could not set the component for Entity ID{entity.ID} in {caller}, {line}");
        //            }
        //        case ErrorType.CouldNotRemoveComp:
        //            {
        //                Console.WriteLine($"Could not remove the component for Entity ID{entity.ID} in {caller}, {line}");
        //                Console.WriteLine($"Stack trace: {st}");
        //                throw new ArgumentException($"Could not remove the component for Entity ID{entity.ID} in {caller}, {line}");
        //            }
        //        case ErrorType.CompDoesNotExist:
        //            {
        //                Console.WriteLine($"There are no chunks of type {component} in {caller}, {line}");
        //                Console.WriteLine($"Stack trace: {st}");
        //                throw new ArgumentException($"There are no chunks of type {component} in {caller}, {line}");
        //            }

        //    }
        //}




        #region DebugFunctions
#if DEBUG
        /// <summary>
        /// Prints existing chunks to the console
        /// </summary>
        public void DEBUG_PrintChunks(bool wait = false)
        {
            if (wait) { Console.WriteLine("Press Enter to continue."); Console.ReadLine(); }
            Console.Clear();
            foreach (var chunk in _chunks)
            {
                chunk.DEBUG_PrintEntities();
            }

            //foreach (var type in _componentChunks.Keys)
            //{
            //    foreach (var chunk in _componentChunks[type])
            //    {
            //        chunk.DEBUG_PrintEntities();
            //    }
            //}
        }
        /// <summary>
        /// Counts entities in all chunks
        /// </summary>
        /// <returns></returns>
        public void DEBUG_CountEntitiesInChunks()
        {
            int total = 0;
            foreach (var chunk in _chunks)
            {
                total += chunk.EntityCount;
            }
            Console.WriteLine($"Counted Entities in chunks: {total}");
        }
#endif
        #endregion
    }
}
