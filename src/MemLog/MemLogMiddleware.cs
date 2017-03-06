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
        private readonly IMemLogService _service;
        public MemLogMiddleware(RequestDelegate next, IMemLogService service)
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
            sb.AppendLine(".crit{background-color: red; color:#fff;}");
            sb.AppendLine(".fail{background-color: red;}");
            sb.AppendLine(".warn{color:yellow;}");
            sb.AppendLine(".info{color:DarkGreen;}");
            sb.AppendLine(".dbug{color:Gray;}");
            sb.AppendLine(".trce{color:Gray;}");

            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<header>MemLog</header>");
            foreach (var item in _service)
            {
                sb.Append("<pre>");
                if (item.LogLevel.HasValue)
                {
                    sb.Append(LogLevelString(item.LogLevel.Value));
                }
                sb.Append(item.Message);
                sb.AppendLine("</pre>");
            }
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private string LogLevelString(LogLevel level)
        {
            string tag = String.Empty;
            switch (level)
            {
                case LogLevel.Trace:
                    tag = "<span class=\"trce\">trce</span>";
                    break;
                case LogLevel.Debug:
                    tag = "<span class=\"dbug\">dbug</span>";
                    break;
                case LogLevel.Information:
                    tag = "<span class=\"info\">info</span>";
                    break;
                case LogLevel.Warning:
                    tag = "<span class=\"warn\">warn</span>";
                    break;
                case LogLevel.Error:
                    tag = "<span class=\"fail\">fail</span>";
                    break;
                case LogLevel.Critical:
                    tag = "<span class=\"crit\">crit</span>";
                    break;
                case LogLevel.None:
                    tag = "<span class=\"trce\">trce</span>";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level));
            }
            return tag;
        }
    }
}
