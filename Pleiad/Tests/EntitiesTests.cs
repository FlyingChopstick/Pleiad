using PleiadEntities;
using System;
using Xunit;

namespace Pleiad
{

    public class EntitiesTests
    {
        public struct TestComponent : IPleiadComponent
        {
            public string testField { get; init; }
        }
        public struct TestComponent2 : IPleiadComponent
        {
            public string testField2 { get; init; }
        }
        EntityTemplate _testTemplate = new(
           new Type[]
           {
                typeof(TestComponent)
           },
           new IPleiadComponent[]
           {
                new TestComponent()
                {
                    testField = "test"
                }
           }
        );


        [Fact]
        public void EntityShouldBeCreated()
        {
            EntityManager em = new EntityManager();

            em.AddEntity(_testTemplate);

            Assert.Equal(1, em.EntityCount);
        }
        [Fact]
        public void EntityShouldBeDeleted()
        {
            EntityManager em = new EntityManager();
            Entity entity = em.AddEntity(_testTemplate);
            em.RemoveEntity(entity);

            Assert.Equal(0, em.EntityCount);
        }
        [Fact]
        public void ChunkShouldBeAdded()
        {
            EntityManager em = new();
            em.AddEntity(_testTemplate);

            Assert.Equal(1, em.ChunkCount);
        }
        [Fact]
        public void EntityDataShouldBeRetrieved()
        {
            EntityManager em = new();
            Entity entity = em.AddEntity(_testTemplate);

            var res = em.GetComponentData<TestComponent>(entity);
            Assert.True(res.IsFound);
            Assert.Equal("test", res.Data.testField);
        }
        [Fact]
        public void EntityDataShouldBeModified()
        {
            EntityManager em = new();
            Entity entity = em.AddEntity(_testTemplate);
            em.SetComponentData(entity, typeof(TestComponent), new TestComponent { testField = "changed" });

            var res2 = em.GetComponentData<TestComponent>(entity);
            Assert.True(res2.IsFound);
            Assert.Equal("changed", res2.Data.testField);
        }
        [Fact]
        public void ComponentShouldBeAdded()
        {
            EntityManager em = new();
            Entity entity = em.AddEntity(_testTemplate);
            em.AddComponent(entity, typeof(TestComponent2), new TestComponent2() { testField2 = "second" });

            var res = em.GetComponentData<TestComponent2>(entity);

            Assert.True(res.IsFound);
            Assert.Equal("second", res.Data.testField2);
        }
        [Fact]
        public void ComponentShouldBeRemoved()
        {
            EntityManager em = new();
            Entity entity = em.AddEntity(_testTemplate);
            var r = (TestComponent)(_testTemplate.ComponentData[0]);
            var res = em.GetComponentData<TestComponent>(entity);

            Assert.True(res.IsFound);
            Assert.Equal(r.testField, res.Data.testField);
        }
    }
}
