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
            var person = ProxyFactory.CreateProxy<IPerson>(new Person()
            {
                Name = "test",
                Parent = new Person(),
            }, t);

            var name = person.Name;
            var parentName = person.Parent.Name;

            Assert.Equal(new Tracer()
            {
                {"name", new Tracer()},
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
    }

    internal class Person : IPerson
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        public IPerson Parent { get; set; }

        public IReadOnlyList<IPerson> Parents { get; set; }
    }

    internal interface IPerson : ITraceable
    {
        string Name { get; }

        IPerson Parent { get; }

        IReadOnlyList<IPerson> Parents { get; }
    }
}
