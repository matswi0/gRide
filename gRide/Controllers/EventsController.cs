using Microsoft.AspNetCore.Mvc;

namespace gRide.Controllers
{
    public class EventsController : Controller
    {
        public IActionResult Create()
        {
            return View();
        }
    }
}
