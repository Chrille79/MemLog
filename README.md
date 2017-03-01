# MemLog
ILogger for Asp.Net Core that let you review the log in a webpage.

![screenshot](assets/MemLog2.png?raw=true)

## Getting Started
```
Install-Package MemLog.ChristianJohansson
```
## Setup
```cs
// Startup.cs
public void ConfigureServices(IServiceCollection services)
{
  //Add MemLogService thats hold the memory log
  services.AddMemLogService();
  services.AddMvc();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, MemLogService memLogService)
{
  loggerFactory.AddConsole();
  //Add MemLog
  loggerFactory.AddMemLog(memLogService, LogLevel.Trace);

  if (env.IsDevelopment())
  {
    app.UseDeveloperExceptionPage();
    //Add MemLogMiddleware to enable /memlog page
    app.UseMemLog();
  }

  app.UseMvcWithDefaultRoute();
}
```
