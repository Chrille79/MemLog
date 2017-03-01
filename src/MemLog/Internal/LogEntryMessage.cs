using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemLog.Internal
{
    public struct LogMessageEntry
    {
        public string LevelString;
        public LogLevel? LogLevel;
        public string Message;
    }
}
