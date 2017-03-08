using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemLog
{
    public class MemLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly MemLogService _service;
        public MemLogMiddleware(RequestDelegate next, MemLogService service)
        {
            _next = next;
            _service = service;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments(new PathString("/memlog")))
            {
                await MakeResponse(httpContext);
                return;
            }

            await _next.Invoke(httpContext);
        }

        private async Task MakeResponse(HttpContext httpContext)
        {
            httpContext.Response.ContentType = "text/html; charset=utf-8";

            var page = Page();
            byte[] data = Encoding.UTF8.GetBytes(page);
            await httpContext.Response.Body.WriteAsync(data, 0, data.Length);
        }

        private string Page()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta charset = \"UTF-8\">");
            sb.AppendLine("<title>MemLog</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body{background-color: #1e1e1e; color:#fff;margin-top: 64px;font-family: Consolas, monospace;}");
            sb.AppendLine("header{height: 56px; background: purple; line-height: 56px; font-size: x-large; position: fixed; top: 0; left: 0; width: 100%; padding-left: 16px;}");
            sb.AppendLine(".crit span {background-color: red; color:#fff;}");
            sb.AppendLine(".crit {border: groove 4px red; padding: 1em 0 1em;}");
            sb.AppendLine(".fail span {background-color: red;}");
            sb.AppendLine(".fail {border: dashed 1px red; padding: 1em 0 1em;}");
            sb.AppendLine(".warn span {color:yellow;}");
            sb.AppendLine(".info span {color:DarkGreen;}");
            sb.AppendLine(".dbug span {color:Gray;}");
            sb.AppendLine(".trce span {color:Gray;}");
            sb.AppendLine("time {color:Gray;}");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<header>MemLog</header>");
            foreach (var item in _service)
            {
                sb.Append("<pre class=\"");
                sb.Append(LogLevelString(item.LogLevel));
                sb.Append("\">");
                sb.Append("<span>");
                sb.Append(LogLevelString(item.LogLevel));
                sb.Append("</span> ");

                sb.Append("<time>");
                sb.Append(item.Time.ToString("O"));
                sb.Append("</time>");
                sb.Append(item.Message);
                sb.AppendLine("</pre>");
            }
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private string LogLevelString(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return "trce";
                case LogLevel.Debug:
                    return "dbug";
                case LogLevel.Information:
                    return "info";
                case LogLevel.Warning:
                    return "warn";
                case LogLevel.Error:
                    return "fail";
                case LogLevel.Critical:
                    return "crit";
                case LogLevel.None:
                    return "trce";
                default:
                    throw new ArgumentOutOfRangeException(nameof(level));
            }
        }
    }
}
