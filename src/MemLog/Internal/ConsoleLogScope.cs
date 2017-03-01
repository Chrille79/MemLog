using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MemLog.Internal
{
    public class ConsoleLogScope
    {
        private readonly string _name;
        private readonly object _state;

        internal ConsoleLogScope(string name, object state)
        {
            _name = name;
            _state = state;
        }

        public ConsoleLogScope Parent { get; private set; }

#if NET451
        private static readonly string FieldKey = $"{typeof(ConsoleLogScope).FullName}.Value.{AppDomain.CurrentDomain.Id}";
        public static ConsoleLogScope Current
        {
            get
            {
                var handle = CallContext.LogicalGetData(FieldKey) as ObjectHandle;
                if (handle == null)
                {
                    return default(ConsoleLogScope);
                }

                return (ConsoleLogScope)handle.Unwrap();
            }
            set
            {
                CallContext.LogicalSetData(FieldKey, new ObjectHandle(value));
            }
        }
#else
        private static AsyncLocal<ConsoleLogScope> _value = new AsyncLocal<ConsoleLogScope>();
        public static ConsoleLogScope Current
        {
            set
            {
                _value.Value = value;
            }
            get
            {
                return _value.Value;
            }
        }
#endif

        public static IDisposable Push(string name, object state)
        {
            var temp = Current;
            Current = new ConsoleLogScope(name, state);
            Current.Parent = temp;

            return new DisposableScope();
        }

        public override string ToString()
        {
            return _state?.ToString();
        }

        private class DisposableScope : IDisposable
        {
            public void Dispose()
            {
                Current = Current.Parent;
            }
        }
    }
}
