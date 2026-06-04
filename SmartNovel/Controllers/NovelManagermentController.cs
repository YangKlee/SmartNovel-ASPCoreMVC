using Microsoft.AspNetCore.Mvc;

namespace SmartNovel.Controllers
{
    public class NovelManagermentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
