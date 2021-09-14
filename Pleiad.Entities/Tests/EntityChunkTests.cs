using System;
using Pleiad.Entities.Components;
using Pleiad.Entities.Model;
using Xunit;

namespace Pleiad.Entities.Tests
{
    public class EntityChunkTests
    {
        private static IEntityChunk GetTestChunk(
            int chunkId = 0,
            Type chunkType = null,
            int chunkSize = EntityChunk.DefaultChunkSize)
        {
            if (chunkType is null)
            {
                chunkType = typeof(NamedComponent);
            }
            return new EntityChunk(chunkId, chunkType, chunkSize);
        }
        private static void AddEntitiesToChunk(IEntityChunk chunk,
            int entityCount = 1,
            IPleiadComponent entityData = default)
        {
            for (int i = 1; i < entityCount + 1; i++)
            {
                chunk.AddEntity(new Entity(i), entityData);
            }
        }
        private static (Entity, IPleiadComponent) GetTestEntity(int entityId = 1)
        {
            return (new Entity(1), GetTestComponent());
        }
        private static NamedComponent GetTestComponent()
        {
            return new NamedComponent
            {
                Name = "Test"
            };
        }


        [Fact]
        public void TestChunkCreate()
        {
            int expectedChunkId = 0;
            Type expectedType = typeof(NamedComponent);
            var entityChunk = GetTestChunk(chunkId: expectedChunkId, chunkType: expectedType);

            Assert.Equal(EntityChunk.DefaultChunkSize, entityChunk.Size);
            Assert.Equal(expectedChunkId, entityChunk.Id);
            Assert.Equal(0, entityChunk.Count);
            Assert.True(entityChunk.IsOpen);
            Assert.Equal(expectedType, entityChunk.ChunkType);
        }

        [Fact]
        public void TestChunkAddEntity()
        {
            var chunkType = typeof(NamedComponent);

            var entity = new Entity(1);
            var entityData = new NamedComponent { Name = "Test" };

            var chunk = GetTestChunk(chunkType: chunkType);
            chunk.AddEntity(entity, entityData);

            Assert.Equal(1, chunk.Count);
            Assert.Equal(EntityChunk.DefaultChunkSize, chunk.Size);
            Assert.True(chunk.IsOpen);
        }

        [Fact]
        public void TestChunkRemoveEntity()
        {
            var chunk = GetTestChunk(0, typeof(NamedComponent));
            AddEntitiesToChunk(chunk, entityData: new NamedComponent { Name = "Test" });
            chunk.RemoveEntity(new Entity(1));

            Assert.Equal(0, chunk.Count);
        }

        [Fact]
        public void TestChunkGetEntityData()
        {
            var chunk = GetTestChunk(chunkType: typeof(NamedComponent));
            (var entity, var entityData) = GetTestEntity();
            chunk.AddEntity(entity, entityData);

            var recievedData = chunk.GetEntityData<NamedComponent>(entity);

            Assert.Equal(entityData, recievedData);
        }

        [Fact]
        public void TestChunkSetEntityData()
        {
            (var entity, var entityData) = GetTestEntity();
            var chunk = GetTestChunk(chunkType: entityData.GetType());

            chunk.AddEntity(entity, entityData);

            var updatedData = GetTestComponent();
            updatedData.Name = "New name";

            chunk.SetEntityData(entity, updatedData);

            var recievedData = chunk.GetEntityData<NamedComponent>(entity);

            Assert.Equal(updatedData, recievedData);
        }
    }
}
