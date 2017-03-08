using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemLog
{
    public class MemLogProvider : ILoggerProvider
    {
        private readonly IMemLogService _service;
        private readonly Func<string, LogLevel, bool> _filter;
        private readonly bool _includeScopes;
        public MemLogProvider(Func<string, LogLevel, bool> filter, bool includeScopes, IMemLogService service)
        {
            _service = service;
            _filter = filter;
            _includeScopes = includeScopes;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new MemLog(categoryName, _filter, _includeScopes, _service);
        }

        public void Dispose()
        {
        }
    }
}
