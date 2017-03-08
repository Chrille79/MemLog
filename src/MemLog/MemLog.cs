using MemLog.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace MemLog
{
    public class MemLog : ILogger
    {
        private static readonly string _loglevelPadding = ": ";
        private readonly IMemLogService _memLogService;

        private Func<string, LogLevel, bool> _filter;

        [ThreadStatic]
        private static StringBuilder _logBuilder;
        
        public MemLog(string name, Func<string, LogLevel, bool> filter, bool includeScopes, IMemLogService memLogService)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Filter = filter ?? ((category, logLevel) => true);
            IncludeScopes = includeScopes;
            _memLogService = memLogService;
        }

        public Func<string, LogLevel, bool> Filter
        {
            get { return _filter; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _filter = value;
            }
        }

        public bool IncludeScopes { get; set; }

        public string Name { get; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (!string.IsNullOrEmpty(message) || exception != null)
            {
                WriteMessage(logLevel, Name, eventId.Id, message, exception);
            }
        }

        private void WriteMessage(LogLevel logLevel, string logName, int eventId, string message, Exception exception)
        {
            var logBuilder = _logBuilder;
            _logBuilder = null;

            if (logBuilder == null)
            {
                logBuilder = new StringBuilder();
            }

            // Example:
            // INFO: ConsoleApp.Program[10]
            //       Request received
            if (!string.IsNullOrEmpty(message))
            {
                // category and event id
                logBuilder.Append(_loglevelPadding);
                logBuilder.Append(logName);
                logBuilder.Append("[");
                logBuilder.Append(eventId);
                logBuilder.AppendLine("]");
                // scope information
                if (IncludeScopes)
                {
                    GetScopeInformation(logBuilder);
                }
                // message
                logBuilder.AppendLine(message);
            }

            // Example:
            // System.InvalidOperationException
            //    at Namespace.Class.Function() in File:line X
            if (exception != null)
            {
                // exception message
                logBuilder.AppendLine(exception.ToString());
            }

            if (logBuilder.Length > 0)
            {
                _memLogService.EnqueueMessage(new LogMessageEntry()
                {
                    Message = logBuilder.ToString(),
                    LogLevel = logLevel,
                    Time = DateTime.Now
                });
            }

            logBuilder.Clear();
            if (logBuilder.Capacity > 1024)
            {
                logBuilder.Capacity = 1024;
            }
            _logBuilder = logBuilder;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return Filter(Name, logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            return ConsoleLogScope.Push(Name, state);
        }

        private void GetScopeInformation(StringBuilder builder)
        {
            var current = ConsoleLogScope.Current;
            string scopeLog = string.Empty;
            var length = builder.Length;

            while (current != null)
            {
                scopeLog = $"=> {current} ";

                builder.Insert(length, scopeLog);
                current = current.Parent;
            }
        }
    }
}
