using MemLog.Internal;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemLog
{
    public class MemLogService : IEnumerable<LogMessageEntry>
    {
        //private readonly BlockingCollection<LogMessageEntry> _que = new BlockingCollection<LogMessageEntry>(10);
        private readonly Queue<LogMessageEntry> _que = new Queue<LogMessageEntry>(100);

        public IEnumerator<LogMessageEntry> GetEnumerator()
        {
            return _que.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _que.GetEnumerator();
        }

        public void EnqueueMessage(LogMessageEntry entry)
        {
            //_que.Add(entry);
            if (_que.Count >= 100)
            {
                _que.Dequeue();
            }

            _que.Enqueue(entry);
            
        }
    }
}
