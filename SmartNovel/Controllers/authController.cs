using Microsoft.AspNetCore.Mvc;

namespace SmartNovel.Controllers
{
    public class authController : Controller
    {
        public IActionResult Index()
        {
            return NotFound();
        }
        [HttpGet]
        public IActionResult Login()
        {
            // show form login chộ này
            return View();
        }
    }
}
