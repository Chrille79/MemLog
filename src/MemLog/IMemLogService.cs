using System.Collections.Generic;
using MemLog.Internal;

namespace MemLog
{
    public interface IMemLogService
    {
        void EnqueueMessage(LogMessageEntry entry);
        IEnumerator<LogMessageEntry> GetEnumerator();
    }
}