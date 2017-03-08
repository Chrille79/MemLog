using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MemLog.TestWeb
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Add MemLogService thats hold the memory log
            services.AddMemLogService();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IMemLogService memLogService)
        {
            loggerFactory.AddConsole();
            //Add MemLog
            //loggerFactory.AddMemLog(memLogService, LogLevel.Trace);
            //Add MemLog - filter namespace Microsoft to LogLevel.Warnings or worse
            loggerFactory.AddMemLog(memLogService,(name, logLevel) => (name.StartsWith("Microsoft") ? logLevel >= LogLevel.Warning : logLevel >= LogLevel.Trace));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //Add MemLogMiddleware to enable /memlog page
                app.UseMemLog();
            }

            app.UseMvcWithDefaultRoute();
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
