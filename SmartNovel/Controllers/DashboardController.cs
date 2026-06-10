using Microsoft.AspNetCore.Mvc;

namespace SmartNovel.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
