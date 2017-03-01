using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MemLog.TestWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _log;
        public HomeController(ILogger<HomeController> log)
        {
            _log = log;
        }
        public IActionResult Index()
        {
            _log.LogInformation("This is a information log entry");
            _log.LogWarning("This is a warning log entry");
            _log.LogError(new EventId(),new Exception(), "This is a error log entry with a exception");


            return View();
        }
    }
}