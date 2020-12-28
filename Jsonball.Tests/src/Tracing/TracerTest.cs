using TFaller.Jsonball.Client.Tracing;
using Xunit;

namespace TFaller.Jsonball.Tests.Client.Tracing
{
    public class TracerTest
    {
        [Fact]
        public void Trace()
        {
            var t = new Tracer();
            t.Trace("a");
            t.Trace("b").Trace("1");

            var expected = new Tracer()
            {
                {"a", new Tracer()},
                {"b", new Tracer()
                    {
                        {"1", new Tracer()},
                    }
                }
            };

            Assert.Equal(expected, t);
        }

        [Fact]
        public void TestPointer()
        {
            Assert.Equal(new string[]{
                "/name",
                "/parent",
                "/parent/name",
                "/special",
                "/special/~0",
                "/special/~1",
            },
            (new Tracer()
            {
                {"name", new Tracer()},
                {"parent", new Tracer()
                    {
                        {"name", new Tracer()}
                    }
                },
                {"special", new Tracer()
                    {
                        {"~", new Tracer()},
                        {"/", new Tracer()}
                    }
                }
            }
            ).ToPointer());
        }
    }
}
