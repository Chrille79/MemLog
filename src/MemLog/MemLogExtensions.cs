using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemLog
{
    public static class MemLogExtensions
    {
        public static IServiceCollection AddMemLogService(this IServiceCollection services)
        {
            return services.AddSingleton<MemLogService>();
        }

        public static ILoggerFactory AddMemLog(this ILoggerFactory factory,
                                          MemLogService memLogService,
                                          Func<string, LogLevel, bool> filter = null)
        {
            factory.AddProvider(new MemLogProvider(filter, true, memLogService));
            return factory;
        }

        public static ILoggerFactory AddMemLog(this ILoggerFactory factory, MemLogService memLogService, LogLevel minLevel)
        {
            return AddMemLog(
                factory,
                memLogService,
                (_, logLevel) => logLevel >= minLevel);
        }

        public static IApplicationBuilder UseMemLog(this IApplicationBuilder app)
        {
            return app.UseMiddleware<MemLogMiddleware>();
        }
    }
}
