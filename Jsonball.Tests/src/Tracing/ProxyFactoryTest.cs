using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using TFaller.Jsonball.Client.Tracing;
using Xunit;

namespace TFaller.Jsonball.Tests.Client.Tracing
{
    public class ProxyFactoryTest
    {
        [Fact]
        public void TestSimpleProxy()
        {
            var t = new Tracer();
            var now = DateTime.Now;

            var person = ProxyFactory.CreateProxy<IPerson>(new Person()
            {
                Name = "test",
                Birthday = now,
                PocketMoney = 12.34M,
                Parent = new Person(),
            }, t);

            var name = person.Name;
            var parentName = person.Parent.Name;

            Assert.Equal(now, person.Birthday);
            Assert.Equal(12.34M, person.PocketMoney);
            Assert.Equal(new Tracer()
            {
                {"name", new Tracer()},
                {"Birthday", new Tracer()},
                {"PocketMoney", new Tracer()},
                {"Parent", new Tracer()
                    {
                        {"name", new Tracer()}
                    }
                }
            }, t);
        }

        [Fact]
        public void TestListProxy()
        {
            var t = new Tracer();

            var person = ProxyFactory.CreateProxy<IPerson>(new Person()
            {
                Parents = new List<IPerson>()
                {
                    new Person(),
                    new Person(),
                }
            }, t);

            // simple access
            var _ = person.Parents[1].Name;

            Assert.Equal(new Tracer()
            {
                {"Parents", new Tracer()
                    {
                        {"1", new Tracer() {
                            {"name", new Tracer()}
                        }}
                    }
                }
            }, t);


            // use enumerator
            foreach (var p in person.Parents)
            {
                var n = p.Name;
            }

            Assert.Equal(new Tracer()
            {
                {"Parents", new Tracer()
                    {
                        {"0", new Tracer()
                            {
                                {"name", new Tracer()}
                            }
                        },
                        {"1", new Tracer()
                            {
                                {"name", new Tracer()}
                            }
                        }
                    }
                }
            }, t);
        }

        [Fact]
        public void TestListenEnum()
        {
            var t = new Tracer();

            var p = PropertyProxy<IPerson>.Create(new Person()
            {
                Gender = Gender.Female
            }, t);

            Assert.Equal<Gender>(Gender.Female, p.Gender);
            Assert.Equal(new Tracer()
            {
                {"Gender", new Tracer()}
            }, t);
        }

        [Fact]
        public void TestNullable()
        {
            var t = new Tracer();
            var person = new Person();

            var p = PropertyProxy<IPerson>.Create(person, t);

            // null nullable
            Assert.Null(p.BirthYear);
            Assert.Equal(new Tracer()
            {
                {"BirthYear", new Tracer()},
            }, t);

            // non null nullable
            t.Clear();
            Assert.Empty(t);

            person.BirthYear = 1970;

            Assert.Equal(1970, p.BirthYear);
            Assert.Equal(new Tracer()
            {
                {"BirthYear", new Tracer()},
            }, t);
        }

        [Fact]
        public void TestGuid()
        {
            var g = Guid.NewGuid();
            var t = new Tracer();
            var proxy = ProxyFactory.CreateProxy<IGuidTest>(new GuidTest() { Id = g }, t);

            Assert.Equal(g, proxy.Id);
            Assert.Equal(new Tracer()
            {
                {"Id", new Tracer()},
            }, t);
        }
    }

    internal class Person : IPerson
    {
        public string Name { get; set; }

        public IPerson Parent { get; set; }

        public IReadOnlyList<IPerson> Parents { get; set; }

        public Gender Gender { get; set; }

        public int? BirthYear { get; set; }

        public DateTime Birthday { get; set; }

        public decimal PocketMoney { get; set; }
    }

    internal interface IPerson : ITraceable
    {
        [JsonPropertyName("name")]
        string Name { get; }

        IPerson Parent { get; }

        IReadOnlyList<IPerson> Parents { get; }

        Gender Gender { get; }

        int? BirthYear { get; }

        DateTime Birthday { get; }

        decimal PocketMoney { get; }
    }

    internal class GuidTest : IGuidTest
    {
        public Guid Id { get; set; }
    }


    internal interface IGuidTest : ITraceable
    {
        Guid Id { get; }
    }

    internal enum Gender
    {
        Male,
        Female
    }
}
