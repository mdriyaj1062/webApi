using CollageApp.MyLogging;
using Microsoft.AspNetCore.Mvc;

namespace CollageApp.Controllers
{
    [Route("api/[controller]")]
    public class DemoController : Controller
    {
        //1. strongly coupled/tightly coupled

        //private readonly IMyLogger _myLogger;
        //public DemoController() 
        //{
        //    _myLogger = new LogToServerMemory(); 

        //}

        // 2. Loosely coupled
     private readonly IMyLogger _myLogger;
     public DemoController(IMyLogger myLogger)
        {
            _myLogger = myLogger;
        }

        [HttpGet] 
        public ActionResult Index()
        {
            _myLogger.Log("Index method started");
            return Ok();
        }
    }
}
