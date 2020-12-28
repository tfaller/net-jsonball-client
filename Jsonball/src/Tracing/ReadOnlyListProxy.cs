using System.Collections;
using System.Collections.Generic;

namespace TFaller.Jsonball.Client.Tracing
{

    /// <summary>
    /// Helper that acts like a proxy for ReadOnlyList.
    /// </summary>
    /// <typeparam name="TValue">Type of the list item</typeparam>
    internal class ReadOnlyListProxy<TValue> : IReadOnlyList<TValue>
    {
        private readonly IReadOnlyList<TValue> _list;
        private readonly Tracer _tracer;

        public ReadOnlyListProxy(IReadOnlyList<TValue> list, Tracer tracer)
        {
            _list = list;
            _tracer = tracer;
        }

        public TValue this[int index]
        {
            get
            {
                var t = _tracer.Trace(index.ToString());
                return (TValue)ProxyFactory.CreateProxy(typeof(TValue), _list[index], t);
            }
        }

        public int Count => _list.Count;

        public IEnumerator<TValue> GetEnumerator() => new Enumerator(_list.GetEnumerator(), _tracer);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class Enumerator : IEnumerator<TValue>
        {
            private IEnumerator<TValue> _enumerator;
            private Tracer _tracer;
            private int _index = -1;

            public Enumerator(IEnumerator<TValue> enumerator, Tracer tracer)
            {
                _enumerator = enumerator;
                _tracer = tracer;
            }

            public TValue Current
            {
                get
                {
                    var t = _tracer.Trace(_index.ToString());
                    return (TValue)ProxyFactory.CreateProxy(typeof(TValue), _enumerator.Current, t);
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                var next = _enumerator.MoveNext();
                if (next)
                {
                    _index++;
                }
                return next;
            }

            public void Reset()
            {
                _enumerator.Reset();
                _index = -1;
            }
        }
    }
}