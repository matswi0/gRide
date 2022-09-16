using Microsoft.AspNetCore.Mvc;

namespace gRide.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
