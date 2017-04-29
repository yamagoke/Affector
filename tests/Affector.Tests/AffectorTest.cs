using Affector;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Affector.Tests
{
    [TestFixture]
    public class AffectorTest
    {
        class EntitySet
        {
            public string FieldValue { get; set; }

            public Entity[] E1 { get; set; }
        }

        [Affectable]
        class Entity
        {
            [Key(0)]
            public string Key1 { get; set; }
            [Key(1)]
            public string Key2 { get; set; }
            [Value("", "V1")]
            public double Value1 { get; set; }
            [Value("", "V2")]
            public double Value2 { get; set; }
        }

        private EntitySet CreateTestEntitySet()
        {
            var e1 = new Entity[]
                {
                    new Entity { Key1 = "A", Key2 = "X", Value1 = 1, Value2 = -1 },
                    new Entity { Key1 = "A", Key2 = "Y", Value1 = 2, Value2 = -2 },
                    new Entity { Key1 = "B", Key2 = "X", Value1 = 3, Value2 = -3 },
                    new Entity { Key1 = "B", Key2 = "Y", Value1 = 4, Value2 = -4 }
                };
            return new EntitySet
            {
                FieldValue = "FieldValue",
                E1 = e1
            };
        }

        private bool Eq(Entity e1, Entity e2)
        {
            return e1.Key1 == e2.Key1 &&
                e1.Key2 == e2.Key2 &&
                e1.Value1 == e2.Value1 &&
                e1.Value2 == e2.Value2;
        }

        [Test]
        public void Generate1()
        {
            var script = "E1(+0.01)";
            var f = Affector.Generate<EntitySet>(script);
            var entitySet = CreateTestEntitySet();
            var actual = f(entitySet);
            actual.FieldValue.Is(entitySet.FieldValue);
            actual.E1.Select(_ => _.Value1).Is(entitySet.E1.Select(_ => _.Value1 + 0.01));
            actual.E1.Select(_ => _.Value2).Is(entitySet.E1.Select(_ => _.Value2 + 0.01));
        }

        [Test]
        public void Generate2()
        {
            var script = "E1(=A, +0.01)";
            var f = Affector.Generate<EntitySet>(script);
            var entitySet = CreateTestEntitySet();
            var actual = f(entitySet);
            actual.FieldValue.Is(entitySet.FieldValue);

            foreach (var i in new[] { 0, 1 })
            {
                Eq(actual.E1[i],
                    new Entity
                    {
                        Key1 = entitySet.E1[i].Key1,
                        Key2 = entitySet.E1[i].Key2,
                        Value1 = entitySet.E1[i].Value1 + 0.01,
                        Value2 = entitySet.E1[i].Value2 + 0.01
                    }).IsTrue();
            }

            foreach (var i in new[] { 2, 3 })
            {
                Eq(actual.E1[i],
                    new Entity
                    {
                        Key1 = entitySet.E1[i].Key1,
                        Key2 = entitySet.E1[i].Key2,
                        Value1 = entitySet.E1[i].Value1,
                        Value2 = entitySet.E1[i].Value2
                    }).IsTrue();
            }
        }

        [Test]
        public void Generate3()
        {
            var script = "E1(=A, !=X, =0.01)";
            var f = Affector.Generate<EntitySet>(script);
            var entitySet = CreateTestEntitySet();
            var actual = f(entitySet);
            actual.FieldValue.Is(entitySet.FieldValue);

            foreach (var i in new[] { 1 })
            {
                Eq(actual.E1[i],
                    new Entity
                    {
                        Key1 = entitySet.E1[i].Key1,
                        Key2 = entitySet.E1[i].Key2,
                        Value1 = 0.01,
                        Value2 = 0.01
                    }).IsTrue();
            }

            foreach (var i in new[] { 0, 2, 3 })
            {
                Eq(actual.E1[i],
                    new Entity
                    {
                        Key1 = entitySet.E1[i].Key1,
                        Key2 = entitySet.E1[i].Key2,
                        Value1 = entitySet.E1[i].Value1,
                        Value2 = entitySet.E1[i].Value2
                    }).IsTrue();
            }

        }

        [Test]
        public void Generate4()
        {
            var script = "E1.V1(+0.01)";
            var f = Affector.Generate<EntitySet>(script);
            var entitySet = CreateTestEntitySet();
            var actual = f(entitySet);
            actual.FieldValue.Is(entitySet.FieldValue);
            actual.E1.Select(_ => _.Value1).Is(entitySet.E1.Select(_ => _.Value1 + 0.01));
            actual.E1.Select(_ => _.Value2).Is(entitySet.E1.Select(_ => _.Value2));
        }

        [Test]
        public void Generate5()
        {
            var script = "E1.V1(=A, +0.01),E1.V2(=A, -0.01)";
            var f = Affector.Generate<EntitySet>(script);
            var entitySet = CreateTestEntitySet();
            var actual = f(entitySet);
            actual.FieldValue.Is(entitySet.FieldValue);

            foreach (var i in new[] { 0, 1 })
            {
                Eq(actual.E1[i],
                    new Entity
                    {
                        Key1 = entitySet.E1[i].Key1,
                        Key2 = entitySet.E1[i].Key2,
                        Value1 = entitySet.E1[i].Value1 + 0.01,
                        Value2 = entitySet.E1[i].Value2 - 0.01
                    }).IsTrue();
            }

            foreach (var i in new[] { 2, 3 })
            {
                Eq(actual.E1[i],
                    new Entity
                    {
                        Key1 = entitySet.E1[i].Key1,
                        Key2 = entitySet.E1[i].Key2,
                        Value1 = entitySet.E1[i].Value1,
                        Value2 = entitySet.E1[i].Value2
                    }).IsTrue();
            }
        }
    }
}
